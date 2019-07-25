
#if NETSTANDARD2_0
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Runtime.Loader;

namespace Beffyman.AspNetCore.Client.Generator
{
	/// <summary>
	/// https://github.com/AArnott/Nerdbank.MSBuildExtension
	/// </summary>
	public abstract class ContextIsolatedTask : Task, ICancelableTask
	{

		/// <summary>
		/// The source of the <see cref="CancellationToken" /> that is canceled when
		/// <see cref="ICancelableTask.Cancel" /> is invoked.
		/// </summary>
		private readonly CancellationTokenSource cts = new CancellationTokenSource();

		/// <summary>Gets a token that is canceled when MSBuild is requesting that we abort.</summary>
		public CancellationToken CancellationToken => this.cts.Token;

		/// <summary>Gets the path to the directory containing managed dependencies.</summary>
		protected virtual string ManagedDllDirectory => Path.GetDirectoryName(new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath);

		/// <summary>
		/// Gets the path to the directory containing native dependencies.
		/// May be null if no native dependencies are required.
		/// </summary>
		protected virtual string UnmanagedDllDirectory => null;

		/// <inheritdoc />
		public void Cancel() => this.cts.Cancel();

		/// <summary>
		/// The body of the Task to execute within the isolation boundary.
		/// </summary>
		protected abstract bool ExecuteIsolated();

		/// <summary>
		/// Loads an assembly with a given name.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly to load.</param>
		/// <returns>The loaded assembly, if one could be found; otherwise <c>null</c>.</returns>
		/// <remarks>
		/// The default implementation searches the <see cref="ManagedDllDirectory"/> folder for
		/// any assembly with a matching simple name.
		/// Derived types may use <see cref="LoadAssemblyByPath(string)"/> to load an assembly
		/// from a given path once some path is found.
		/// </remarks>
		protected virtual Assembly LoadAssemblyByName(AssemblyName assemblyName)
		{
			if (assemblyName.Name.StartsWith("Microsoft.Build", StringComparison.OrdinalIgnoreCase) ||
				assemblyName.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
			{
				// MSBuild and System.* make up our exchange types. So don't load them in this LoadContext.
				// We need to inherit them from the default load context.
				return null;
			}

			string assemblyPath = Path.Combine(this.ManagedDllDirectory, assemblyName.Name) + ".dll";
			if (File.Exists(assemblyPath))
			{
				return this.LoadAssemblyByPath(assemblyPath);
			}

			return null;
		}

		/// <summary>
		/// The context the inner task is loaded within.
		/// </summary>
		private AssemblyLoadContext ctxt;

		/// <inheritdoc />
		public sealed override bool Execute()
		{
			try
			{
				string taskAssemblyPath = new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath;
				this.ctxt = new CustomAssemblyLoader(this);
				Assembly inContextAssembly = this.ctxt.LoadFromAssemblyPath(taskAssemblyPath);
				Type innerTaskType = inContextAssembly.GetType(this.GetType().FullName);

				object innerTask = Activator.CreateInstance(innerTaskType);
				return this.ExecuteInnerTask(innerTask);
			}
			catch (OperationCanceledException)
			{
				this.Log.LogMessage(MessageImportance.High, "Canceled.");
				return false;
			}
		}

		/// <summary>
		/// Loads the assembly at the specified path within the isolated context.
		/// </summary>
		/// <param name="assemblyPath">The path to the assembly to be loaded.</param>
		/// <returns>The loaded assembly.</returns>
		protected Assembly LoadAssemblyByPath(string assemblyPath)
		{
			if (this.ctxt == null)
			{
				throw new InvalidOperationException("AssemblyLoadContext must be set first.");
			}

			return this.ctxt.LoadFromAssemblyPath(assemblyPath);
		}

		private bool ExecuteInnerTask(object innerTask)
		{
			Type innerTaskType = innerTask.GetType();
			Type innerTaskBaseType = innerTaskType;
			while (innerTaskBaseType.FullName != typeof(ContextIsolatedTask).FullName)
			{
				innerTaskBaseType = innerTaskBaseType.GetTypeInfo().BaseType;
			}

			var outerProperties = this.GetType().GetRuntimeProperties().ToDictionary(i => i.Name);
			var innerProperties = innerTaskType.GetRuntimeProperties().ToDictionary(i => i.Name);
			var propertiesDiscovery = from outerProperty in outerProperties.Values
									  where outerProperty.SetMethod != null && outerProperty.GetMethod != null
									  let innerProperty = innerProperties[outerProperty.Name]
									  select new { outerProperty, innerProperty };
			var propertiesMap = propertiesDiscovery.ToArray();
			var outputPropertiesMap = propertiesMap.Where(pair => pair.outerProperty.GetCustomAttribute<OutputAttribute>() != null).ToArray();

			foreach (var propertyPair in propertiesMap)
			{
				object outerPropertyValue = propertyPair.outerProperty.GetValue(this);
				propertyPair.innerProperty.SetValue(innerTask, outerPropertyValue);
			}

			// Forward any cancellation requests
			MethodInfo innerCancelMethod = innerTaskType.GetMethod(nameof(Cancel));
			using (this.CancellationToken.Register(() => innerCancelMethod.Invoke(innerTask, new object[0])))
			{
				this.CancellationToken.ThrowIfCancellationRequested();

				// Execute the inner task.
				var executeInnerMethod = innerTaskType.GetMethod(nameof(ExecuteIsolated), BindingFlags.Instance | BindingFlags.NonPublic);
				bool result = (bool)executeInnerMethod.Invoke(innerTask, new object[0]);

				// Retrieve any output properties.
				foreach (var propertyPair in outputPropertiesMap)
				{
					propertyPair.outerProperty.SetValue(this, propertyPair.innerProperty.GetValue(innerTask));
				}

				return result;
			}
		}

		private class CustomAssemblyLoader : AssemblyLoadContext
		{
			private readonly ContextIsolatedTask loaderTask;

			internal CustomAssemblyLoader(ContextIsolatedTask loaderTask)
			{
				this.loaderTask = loaderTask;
			}

			protected override Assembly Load(AssemblyName assemblyName)
			{
				return this.loaderTask.LoadAssemblyByName(assemblyName)
					?? Default.LoadFromAssemblyName(assemblyName);
			}

			protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
			{
				string unmanagedDllPath = Directory.EnumerateFiles(
					this.loaderTask.UnmanagedDllDirectory,
					$"{unmanagedDllName}.*").Concat(
						Directory.EnumerateFiles(
							this.loaderTask.UnmanagedDllDirectory,
							$"lib{unmanagedDllName}.*"))
					.FirstOrDefault();
				if (unmanagedDllPath != null)
				{
					return this.LoadUnmanagedDllFromPath(unmanagedDllPath);
				}

				return base.LoadUnmanagedDll(unmanagedDllName);
			}
		}

	}
}

#endif

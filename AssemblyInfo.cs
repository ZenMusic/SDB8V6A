// Step-by-step plan:
// 1. Automatically increment the version number before publishing.
// 2. Ensure each publish uses a unique version.
// 3. Use the following AssemblyInfo.cs pattern for auto-incrementing the build/revision numbers.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Major.Minor.Build.Revision
// The '*' will auto-increment Build and Revision on each build.
[assembly: AssemblyVersion("2.0.0.*")]
[assembly: AssemblyFileVersion("2.0.0.0")]
< PropertyGroup >
  < Version > 3.0.0.*</ Version >
  < AssemblyVersion > 2.0.0.*</ AssemblyVersion >
  < FileVersion > 2.0.0.0 </ FileVersion >
</ PropertyGroup >

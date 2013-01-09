using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Core")]
[assembly: AssemblyCopyright("Copyright ©  2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7c69b4e2-c77d-441d-af12-2560f07711f1")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if TEST
[assembly: InternalsVisibleTo("Kinectitude.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100732d112ee14b15" +
                                                    "1a0ec09f2ee6eb0b6d504cf49d57d9d9d78161e6647a4879e1b01d9ef604fd51e150ca7e7b75a9" +
                                                    "d54d20a9fb987bca2e51bbf0dae003ee50c88d86c823a18a4bb3e37a459acb705714e49f66f910" +
                                                    "a7e2ed9dfcab6856c316b4234ea47bd30076bc7fc41d1a43c2de6810a45b06517fed7e20f1360093accff6")]
#endif

[assembly: InternalsVisibleTo("Kinectitude.Player, PublicKey=0024000004800000940000000602000000240000525341310004000001000100732d112ee14b15" +
                                                    "1a0ec09f2ee6eb0b6d504cf49d57d9d9d78161e6647a4879e1b01d9ef604fd51e150ca7e7b75a9" +
                                                    "d54d20a9fb987bca2e51bbf0dae003ee50c88d86c823a18a4bb3e37a459acb705714e49f66f910" +
                                                    "a7e2ed9dfcab6856c316b4234ea47bd30076bc7fc41d1a43c2de6810a45b06517fed7e20f1360093accff6")]

[assembly: InternalsVisibleTo("Kinectitude.Editor, PublicKey=0024000004800000940000000602000000240000525341310004000001000100732d112ee14b15" +
                                                    "1a0ec09f2ee6eb0b6d504cf49d57d9d9d78161e6647a4879e1b01d9ef604fd51e150ca7e7b75a9" +
                                                    "d54d20a9fb987bca2e51bbf0dae003ee50c88d86c823a18a4bb3e37a459acb705714e49f66f910" +
                                                    "a7e2ed9dfcab6856c316b4234ea47bd30076bc7fc41d1a43c2de6810a45b06517fed7e20f1360093accff6")]
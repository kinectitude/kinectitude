Kinectitude
===========

Kinectitude is an extensible framework and GUI editor for building simple 2D arcade games with the Kinect sensor.

Introduction
------------

To get started using Kinectitude, you will need to build it from the source. Here's a breakdown of the directories in this repository:

* Libraries/ Contains some third-party dependencies Kinectitude needs to run
* Samples/ Contains example Kinectitude projects with KGL files and image assets that can be opened in the editor
* Source/ Contains the source code for all projects that make up Kinectitude

Step 1: Install Dependencies
----------------------------

Kinectitude uses some third-party libraries. While some of these libraries are included in this repository, the following ones need to be downloaded and installed separately in order to develop Kinectitude:

* [SlimDX January 2012 SDK](http://slimdx.org/download.php)
* [Kinect for Windows SDK 1.6 or newer](http://www.microsoft.com/en-us/kinectforwindows/develop/developer-downloads.aspx)
* [Microsoft Expression Blend SDK for .NET 4](http://www.microsoft.com/en-ca/download/details.aspx?id=10801)

Once both of these SDKs are installed, you can get started by building the Kinectitude solutions!

Step 2: Build the Solutions
---------------------------

To build Kinectitude you will need some flavor of Visual Studio 2012. The free Visual Studio Express 2012 for Windows Desktop should get the job done. There are two main solutions files in this repository:

* Source/Kinectitude.Player.sln
* Source/Kinectitude.Editor.sln

The first solution contains the Kinectitude Interpreter (the "Core") project, the Kinectitude Player project, as well as projects for all of the supporting plugins (Input, Kinect, Physics, and Rendering). The second solution contains the Editor project. Open each solution in Visual Studio, and choose your preferred build configuration. You may choose Debug or Release configurations depending on whether you are developing or not, but in all cases we recommend Any CPU as the target platform. Building either solution will create a Build/ folder in the root of your copy of the repository. The executables and DLLs will be placed in a subfolder of Build/ named after the configuration and platform you have chosen. 

Step 3: Run the Tests
---------------------

The test solution can be found in Source/Kinectitude.Tests.sln. Open it up in Visual Studio, and ensure the build configuration is set to Test. To start the unit tests, choose Test > Run Tests > Run All Tests from the menu. Visual Studio will build the test project and run all of the unit tests. 
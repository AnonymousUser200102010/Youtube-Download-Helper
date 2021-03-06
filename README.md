Youtube Download Helper
==========
Youtube Download Helper is a program whose main purpose is to batch download Youtube videos without all the trouble of most free options and the extortionate tactics of some paid options.

_This program was designed using the .Net Framework, Version 4.5.1, and using C# Compiler Version 5.0_

#Licenses
[First Party]:

Youtube Download Helper is licensed under the GNU General Public License.  see <http://www.gnu.org/licenses/> for more information.

[Third Party]:

[Youtube Extractor] (https://github.com/flagbug/YoutubeExtractor "https://github.com/flagbug/YoutubeExtractor").

[Components:

* The YouTube URL-extraction code is licensed under the [MIT License] (http://opensource.org/licenses/MIT "http://opensource.org/licenses/MIT").

* The audio extraction code that is originally from [FlvExtract] (http://moitah.net/ "http://moitah.net/") is licensed under the [GNU General Public License version 2 (GPLv2)] (http://opensource.org/licenses/gpl-2.0 "http://opensource.org/licenses/gpl-2.0").]

[Xceed WPF Toolkit] (http://wpftoolkit.codeplex.com "http://wpftoolkit.codeplex.com") is licensed under the [Microsoft Public License (Ms-PL)] (http://wpftoolkit.codeplex.com/license "http://wpftoolkit.codeplex.com/license").

[Fody] (https://github.com/Fody/Fody/ "https://github.com/Fody/Fody/") and [Costura.Fody] (https://github.com/Fody/Costura "https://github.com/Fody/Costura") are licensed under the [MIT License] (http://opensource.org/licenses/MIT "http://opensource.org/licenses/MIT").

The [Microsoft API Code Pack Shell] (https://www.nuget.org/packages/Windows7APICodePack-Shell/ "https://www.nuget.org/packages/Windows7APICodePack-Shell/") and the [Microsoft API Code Pack Core] (https://www.nuget.org/packages/Windows7APICodePack-Core/ "https://www.nuget.org/packages/Windows7APICodePack-Core/") are both depreciated, and I cannot find their licenses. So if anyone knows please make me aware as soon as possible.

The [HTML Agility Pack] (https://htmlagilitypack.codeplex.com/ "https://htmlagilitypack.codeplex.com/") is licensed under the [Microsoft Public License (Ms-PL)] (http://wpftoolkit.codeplex.com/license "http://wpftoolkit.codeplex.com/license").

#Planned Features
Planned features are (in no specific order):

* Additional resolution support for Mp4 and other restricted formats, and additional audio track type support.

* Multi-threading (so multiple downloads can be going at once. At no time will accelerator-style functions be implemented. For stability reasons).

* Scheduling support.

* HD resolution support

* More supported operating systems. Currently only Windows is supported.

Hopefully more to come!

#Use
1. Open in a program with Windows Forms support (I recommend SharpDevelop) and compile/build. A pre-compiled EXE is included for convenience.

2. It should go without saying, but you'll need .Net 4.5.1 or above installed and be using an OS with .Net framework, C# 5.0, and WPF support. If you're having trouble with the pre-compiled version, it was compiled with all major and minor updates for the .Net framework and C# compiler installed on my system. So try installing those.

2a. Also, if using Windows, make sure the main executable YoutubeDownloadHelper.exe, or similar, is run as administrator. The program requires writing to the registry, which requires administrative control.

3. Copy everything in the compiled program folder to a new folder (or not). Make sure this program AND any other required elements are INSIDE their own folder unless you know what you're doing! Create a shortcut to the program (with any desired arguments) and run!

_Usage within the program is fairly self-explanatory._

#Disclaimer
**I am not liable for any damage done to you, your property, or otherwise with regard to the running, compiling, and/or overall use of this program, it's associated libraries (if any), and/or any additional data contained within the source. I am likewise not financially, morally or legally obligated to pay for the cost of the aforementioned property, any lost wages, any emotional distress, and/or the repairs thereof.**

**This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.**

**I do not extend additional warranties and will not fulfil any warranty given to you by any third party or otherwise. By downloading this source, compiling the source from either github, a third party website, or otherwise, and/or using (a) pre-compiled version(s) of this program from a third party website or otherwise, you agree to not only the license contained within this project, regardless of whether your version contained said license, but the information within this disclaimer and agree to all said information from the point you downloaded forward. _If using a previous version you agree to any new additions or removals of/to said information_.**

**This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.**
 
**All users and/or distributors of the project who either plan to use this source in a fork, within a separate project, and/or to pre-package and give to others (provided said use does not go against the license contained within), are _strongly_ encouraged to add this disclaimer to any and all licenses/disclaimers contained within the product you are using, producing, and/or distributing so that the end-user is aware of said disclaimer. _I, again, am not liable for any damages related to said use_. _As is outlined in this disclaimer_.**
# Utitshala
A chat engine -based LMS with a unique semantic language for Learning Design.

# How it Works
Utitshala delivers learning content through Telegram, using the @UtitshalaBot. Utitshala is based on an event architecture. This means that messages are received via the @UtitshalaBot, computed in ChatEngine.cs, and executed to provide some sort of response (such as sending a learning design activity). 

The ChatEngine.cs file relies on Spin, a dialogue library written by Sam Bloomberg (https://github.com/redxdev/Spin). As such, learning content is structured in a sequence format, with activity elements that lead to other elements based on user input. Utitshala extends Spin to include a variety of what it calls 'sequence commands'. These commands form the structure of the Learning Design semantic language, and lead to specific chat outputs. For example, a learning design with a 'presentation' sequence command sends an album of images to the learner over Telegram, similar to a real-life presentation.

The sequence commands in this language include:

* next: sets the next activity element to send.
* opt: presents the learner with options, each leading to a different activity element.
* image: sends an image file, either compressed or uncompressed by the engine.
* sticker: sends a Telegram sticker.
* presentation: as above, sends an album of images, compressed or uncompressed.
* wait: holds the chat for a specific amount of time.
* input: receives input from the user for some purpose, such as when choosing an activity. Supports several forms of Regex.
* execute: executes one of several internal functions, such as checking if a user is registered.
* upload: prepares to receive a file and save it into the app's internal directories.
* forum: hosts a forum between learners, tracked asynchronously on a database table.

Utitshala also includes an ASP.Net MVC site, with support for schools and classrooms that learners may join. A primitive learning design editor is present, but requires expansion. The entity class model is provided within this git in ClassDiagram.cd.

## Setup
After cloning the repo, open the Package Manager Console and run the following commands:

`Add-Migration InitialCreate`

`Update-Database`

If you see the error:

`Directory lookup for the file "C:**\Utitshala\Utitshala\App_Data\aspnet-Utitshala-20210518051037.mdf" failed with the operating system error 2(The system cannot find the file specified.).
CREATE DATABASE failed. Some file names listed could not be created. Check related errors.`

Open `Utitshala.csproj` and remove the following line: `<Folder Include="App_Data\" />`

Then re-add the App_Data folder to the project. 

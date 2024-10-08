<img src="https://img.shields.io/github/v/release/ZeynelKoca/LeagueOfLegends-int-list.svg">

# Sisko's LeagueOfLegends Int List
An external application that makes use of the official League of Legends API to create an int list. The application saves all summoners in a file and can even delete them by putting 
a `-` in front of the summoner name. When active, it will read the League of Legends client process and once in champion select, will go through your teammates' summoner names.


When a player is found to be in your int list, the application will pop-up to the front screen and also make a quick "pop" sound. It will then list the summoner(s) that have been
found to be in your int list. 

<p align="center">
  <img src="IntList/Resources/Application.png">
</p>


## Usage
- After opening the application for the first time, you have to set the League of Legends folder. Most of the time this is located in the C: drive under Program Files.
- You add people to your int list by typing their summoner name in the white bar and by pressing the "Add" button.
- You remove people from your int list by typing their summoner name with a `-` in front and by pressing the "Add" button.
- Loading into a game will automatically trigger the application to go through the int list process. If it finds someone in your int list, it will pop-up with a sound effect to let you know. Simply keep the application running while you play the game and it will handle the rest.

Author: Ana Figueira

BallAndRock
===========

Ball and Rock

The game is played with the mobile phone in portrait mode. The game should have a ball that rolls on a flat surface (the bottom of the screen). If the player rotates his phone to the left or right, the ball must move accordingly simulating the gravity pull. Also, the ball movement should take into account acceleration and deceleration. So, if I rotate the phone to the right and the ball starts rolling to the right, if I then rotate to the left, the ball moving speed should decrease until it stops and just then should it start to roll to the left at an increasing speed.

The edges of the screen serve as walls with which the ball collides.  

At the same time, rocks are falling from the top to the bottom of the screen with random X coordinates. The rocks are immune to the gravity pull and, as such, keep a constant X coordinate throughout their fall.

If the stones and the ball collide, the player loses.

Game implementation details:
============================

- Ball movement takes into consideration acceleration and deacceleration of the ball.
- Rocks are placed at random positions at the top of the screen. 
- Rock images are randomly selected.
- Rocks are placed at random times and the number of rocks increases until the specified number of rocks.
- Animations (sprites) for the rocks and ball were added.
- Sounds were added that indicate when a rock leaves the screen (point for the user) and for when a rock collides with the ball.


Code Instructions:
==================

The target system of the application is Windows Phone 8.1. 
In order to run the code, the following are required:
- Windows 8.1 needs to be installed in the machine
- Visual Studio 2013 with Update 3 (Windows Phone SDK 8.1) also.
- In order to test the application in a device, this must be updated to Windows Phone 8.1. 


The code has been tested on a physical device (Nokia Lumia 720 with Windows Phone 8.1) and no problems were found.
It was also tested using the "Windows App Certification Kit" and the test results can be found in the folder AppPackages. It passed all tests. 

The .appxbundle package is also in the folder AppPackages.

Folder with the package and test results wasn't being uploaded to github, so it is zipped as the file AppPackages.zip. 

This project can be found hosted on:

https://github.com/AnaFigueira/BallAndRock 


Thanks for the fun!
Enjoy!
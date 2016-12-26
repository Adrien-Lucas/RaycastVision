# RaycastVision
A simple game based on the raycasting projection system of the firsts 3d games like Wolfenstein3D or Doom.

![Screen](http://image.prntscr.com/image/7b458a5b8a0d4c82ab63df39481e707e.png)

## Play

* Download the project
* Go to RaycastVision\bin\x86\Debug
* Launch RaycastVision.exe

## The game

* Z,Q,S,D   Move (Sorry qwerty)
* Space     Shot
* R         Reload

The graphical ressources come from Wolfenstein3D.
They are easy to find on a image browser.

There is no goal to the game since it's not a game but a technical demo, so you can only move and kill some nazis.

## The principle of Raycasting

Raycasting was the first technology that gives possibilities of runtime 3D. Rays are sended on the field of view of the character, the rays are sended on the map grid. When a ray hits a wall, it render a line on the screen with the texture of the wall and with a height that depends on the distances of the wall from the character.

### Define a grid

The game receives a png image that is a map, its only an image with some colored pixels.
![Map](http://image.prntscr.com/image/641c320bfb404e6cb6d51b3fb26d51af.png)

We give some properties to each colors.
* Black is a simple brick wall
* Red is a wall with a nazi flag
* Brown is an imperial eagle statue
* Blue is the initial position of the character
* Green is an enemy
* The other colors are for objects like barrels or lamps.

Each color that is not a wall is replaced by white on the grid.

### Raycasting

A circle equation is defined to represent the FOV. This circle is used only on the part defined by the rotation and the FOV (at 30 by default). The more points are on this part of circle, the more the game will be precise on the horizontal rendering.

Then a ray is sent from the character to one point of the sight of view, a point begin at the character and its position is increased of a 'step' amount, the more 'step' is near of 0, the more the vertical rendering would be nice. 

This point checks the grid at each of its positions and if it is on a point of the grid, it stop and send back the position of the encountered object. Then a line is rendered with a height based on the distance from the character to the collision point. This line is at a position defined by the ratio (btw 0 & 1) of the tested point on the total points to test.

This operation is repeted on each of the points that define the sight of view of the character.

## Conclusion

This was a very intersting exercise that helps me to understand how XNA works but also gives me the possibility of "re-inventing the wheel" as I tried to re-invent the raycasting process by not watching docs on the net.

The result is not perfect and their still optimization work to do, but I'm pretty proud of what I've done and I think this is the most important as long as I do it only for myself and not for school or a boss.

Thanks for reading me :)

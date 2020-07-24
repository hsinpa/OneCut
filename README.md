# Installation
* Drag OneCut.cs to any gameobject, but remember to add it once because it is a singleton.
* Drag OneCutObject to gameobject you want to perform actual cutting. Not to apply it to every gameobject if you want better performance.

# Class Detail
* SpriteCutObject
* It will copy the sprite render you attach on, so once cutting is perform, it won’t affect the original sprite.

## CutResult
Include all the information need after cutting<br/>
  * mainSprite => Mesh with larger area after cutting
  * subSprite => Mesh with lesser area
  * originalSprite => Mesh before cutting
  * intersectionPoints => Debug usage, see where your cut line intersect with original mesh

## One cut
  * Singleton
  * Check debugMode, if you want to see where your cut take place
  * Method Cut is work under threading to prevent blocking ui thread


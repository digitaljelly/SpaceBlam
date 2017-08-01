Notices:

Excepting the following, all content & code was created by me:
	- Skybox texture
	- UI visuals (I've use the standard Unity UI for now)
	- Imported Fonts
	- Imported Audio files (music, and SFX)
	- Some camera effects (Bloom & Chromatic Abberation from the Standard Assets library)

Game brief:
	"Asteroids-like game."
	I decided to do an eternal shooter whereby asteroids are replenished as you destroy them. 
	The aim is to beat the previous high score.
	
	In addition, I've taken the opportunity to work in some spaceship customization
	and add in some custom shaders (1x surface shader and 1x fragment shader).
	
Programming approach:
	The main challenge was the tiling of elements as they go off-screen (toroidal-style).
	There were three possible approaches:
		1) Simple - as positions go outside screen bounds, snap to the opposite side
			Issues:
				Horrible snapping - jarring and can move asteroids on top of other elements
		2) Fully dynamic - instantiates/destroyes as game objects join/left the scene
			Issues:
				Complicated - need to work out where objects are leaving/entering the scene
				and work out when and where to instantiate duplicates.
				Tracking everything in a performant manner.
		3) Pre-tiles - 9 copies of each object
			Issues:
				Least performant option.
				Makes instantiating new asteroids complex
	In the end, I settled with 3, given it provided the most predictable results and from a code
	perspective, it wasn't going to end up being too time-consuming.
	Care was taken to avoid too much code running on Update and FixedUpdate to reduce any performance
	issues that running 9x asteroids and players might cause.
	
	The second challenge was creating an infinite game. Asteroids needed to be spawned 
	off-screen, but not interact with off-screen elements that may have resulted in unexpected 
	results for the player. Using a combination of physics layers, it was possible to get around this
	issue.

	In terms of where to go next, I would consider the following steps:
		Refactoring into approach 2)
			- Although a significant amount of work, it would definitely be the best
			compromise between performance and visual sycnronization. I would approach
			this by creating a set of colliders that lined up with the view area. On an
			asteroid touching the edge of the screen, spawn another on the opposite side
			and fix the joins.
			Care could need to be taken to ensure that we spawn appropriately - as it is possible
			to have 1/4 of an asteroid in each corner, so we need to know what edges have been crossed
			and in what order.
		More Customization
			Building on the two tone, it would be nice to add in different bullet colours, thrust
			colours, etc.
		Upgrades
			As the game is infinite in length, it would be nice to provide power-ups and upgrades to the player
			once they have died - either by a "collecting coins" or "spending points" mechanic. Such upgrades
			might include other weapons, shields (which might require a player physics rework).
# Planet

This project is from back in 2016, I was studying computer graphics and did this as a side-project for learning procedural mesh 
generation and shaders.

It's based on a cube projected onto a sphere. Thus each of the six faces is represented as a deformed quad basically.
This made it easy to implement a QuadTree so each face can be split into four new faces via an editor-script as shown in this gif:

https://gfycat.com/gifs/detail/InfantileShyEagle

I previously worked on using heightmaps and noise to project a grid upwards. Here I 
used a similar approach but used the normal direction of each vertex as the displacement vector:

https://gfycat.com/gifs/detail/FlatCaringBlackfootedferret

At this point I discovered the tesselation-pipline, so I learned a lot about that towards the end of this project.

The tesselation api Unity exposed through its surface-shaders had some limitations. If I remember correctly I couldn't pass
down attributes on the newly created vertices into the surface-function. So I started working on Hull/Domain shaders for better control.

I wanted to build tesselated water so I could try using high-frequency noise to recreate some of the surface details water typically has.

So the Hull/Domain shader distorts the vertices of the water and this information is then used in the fragment shader for distorting
the grabpass.

https://gfycat.com/gifs/detail/WanSatisfiedElectriceel

For the foam where the water meets the terrain I ended up using and learning about the depth-buffer. Each fragment checks the depth buffer
to see if terrain has been rendered within a certain radius of this fragment - if so the water fragment becomes white. 
Distorting the radius of this imaginary sphere would probably give more interesting results since the foam always has the same thickness
in this implementation.


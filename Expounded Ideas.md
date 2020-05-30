Mesh and Rendering:
The mesh can be divided up into several parths:
vertices
faces
rendering
noise (simplex?)
optimisation
materials

the gist:
Point voxels are a method of drawing intricate 3d worlds, however,
the use of thousand of voxels is inefficient. By using a combination
of noise map and height map it is possible to create a surface(hm)
and populate the underground with cave systems(3d noise). These
two noise maps can be combined to form a new map that contains
the height map and the noise map. These maps contain points with
values between 0 and 1. The points, or cells, of this composite map
can then be used to tell the system that there is or is not something
in that cell. 
By combining this method of determining whether or not something
is in a cell, with a method of rendering everything in a field of
view, it is possible to create a tertiary system.
This tertiary system will take the points in the field of view and
form a mesh, or a series of meshes (if they are not connected). This
new mesh would then count as a single mesh with many sides as opposed
to multiple meshes with a set number of sides. This method, if done
correctly, should make the terrain incredibly optimised.
The downside of this method is that materials do not interact well
with composite meshes, a possible fix is to make a series of meshes
grouped by material, or, it can be handled by a shader.


Forces and Density:
To move through air or on land requires several components:
Friction - when moving on land
Force and surface area - when swimming or flying
Density - also when swimming or flying

The gist:
The shape of any one creature is forged by the environment it is in.
For fish they have tails and fins designed to push against water
and propel themselves forward. This design means there is a remarkably
small 'wing span' to push against a liquid with the density of water.
However if you were to make water less dense you would get something
more akin to air, where a larger wing-span would be of a greater advantage.
Additionally, one of the differences between swimming and flying
is the direction of the forces employed. Sea creatures normally 
direct the force behind them, whereas birds would direct it straight
down. This is in part due to gravity, a part due to buoyancy and a
dash of density.
The focus of this part of the project is to accurately describe
the direction of the forces and how they interact with speed, gravity
and density. By creating adapting creatures designed around these
concepts and different sources of energy, it is possible to create
many unique and wonderful designs. In addition this would give greater
breadth to the manipulation of the in game project.

The force required for a bird to fly with a certain wingspan is X
newtons of force acting in a direction against gravity. For a human
it would be an order of magnitude more expensive, but not impossible
through the manipulation of force.


Local Space:
Gravity
orbit

The maths behind gravity is common knowledge by this point, however
it has not been described in a computer program as a series of local
spaces. For this project imagine this:
A body of mass with a large gravitational field, that affects
"local space", a set of x,y,z coordinates with gravitational force
applied to all within it. By itself it is not any different from
the traditional "world space" that is normally employed. The fun 
part of this method is what happens to the space when gravity increases
or decreases and what interactions it will have with neighbouring
gravitational fields. To explain this as an example it first needs
a coefficient, X, X is the standard unit of space. With a gravity
the same as earth, X can be 1. However if gravity were to increase
then so would X, the local space of earth at this point can be
X times greater, or smaller, than it was orignally. Now imagine a blackhole
and its local space grid, multiplied by its strength of gravity, X,
and another version of earth with value Y. The local space of earth
is 1 and the local space of a black hole is X (y * a very high number).
by traversing the local space of X by a unit of 1, you will have
travelled the local space of Y by X. 

By introducing a planetary system and the ability to manipulate 
gravity of an object and introducing its own "local space" it would
create the ability to instantly travel between two points by traversing
across local spaces. Similar to what happens on the edge of a black
hole in reality


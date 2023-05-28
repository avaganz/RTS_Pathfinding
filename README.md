# RTS Pathfinding

Please implement unit pathfinding solution known from classic real time strategy games using Unreal Engine,
Unity or any engine of your choice.

Requirements
a) Map is a two dimensional, 256x256 grid made of tiles of width t which can be either blocked or walkable
b) Units are circles of radius r = t/4 that can move in any direction. Units should not be able to enter a
blocked tile. They should move with speed v = 2t/s
c) Units should not overlap each other, even though some minimal overlap is acceptable. Implement an algo-
rithm that maintains a distance between units.
d ) User should be able to select units with a rectangle and issue move order to them
e) Units should stop once they reach a destination point as a group
f ) User should be able to paint and erase blocked tiles on a map. Units should adjust their path accordingly.
g) Your implementation should handle at least 100 moving units on a grid of size 256x256 with playable
framerate - above 30 fps on a modern hardware

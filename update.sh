#!/bin/sh

# Note that this needs to be updated to work on any system.
# Use this to update all of the scripts out of Rebound Velocity or another project.

PROJECT=~/UnityProjects/Pulsar/
TARGET1=Assets/Scripts/Enemies/Base/ProjectileArc/
TARGET2=Assets/Scripts/General/Base/Activator.cs

cp -r $PROJECT$TARGET1 .
cp -r $PROJECT$TARGET2 .


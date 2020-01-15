# Keyframe Bicubic Interpolation in Unity Engine 2018.4 (Procedural Animation)
![woah!](https://github.com/wellwhy/pent-up/blob/master/pntup.gif?raw=true)

This project is aiming to emulate [Overgrowth's](https://www.youtube.com/watch?v=LNidsMesxSE) animation system. It has reached a functional stage and I will continue to enhance the system to be used for any of my future game projects. The idea is that with only a **minimal amount of keyframes, interpolation of these frames creates a visual result that is almost identical to hand-keyed animation.**

This project **also** implements a **character and camera controller**. Using a variety of overlapping acceleration curves, much thought has been put into different kinds of turns and acceleration in order to create a unique looking and responsive system. This is modeled after [Metal Gear Solid V's](https://www.youtube.com/watch?v=C2A8vxSFWto) animation system.

### What results with this system?
* Exponentially easier and faster creation of animation
* In-engine editable facets of animation such as interpolation, easy specific-bone animation blending, easy animation retargeting, responsive variables (springiness, dampening), and easy animation-matching with a ragdoll

### How did I do this?
1. Modeled a character and rigged it with IK.

![woah!!](https://github.com/wellwhy/pent-up/blob/master/1.png?raw=true)

2. For walking and running, I keyframed a **pass and reach pose (4 poses in total)** and exported them with the model.

![woah!!!](https://github.com/wellwhy/pent-up/blob/master/2.gif?raw=true)

3. Developed a script "serializePose" which applies these keyframes in-engine and **captures their local positions and rotations in relation to each other (to a JSON) in this format:**
````
{
    "boneOffsets": [
        {
            "localPosition": {
                "x": 0.0,
                "y": 1.032294750213623,
                "z": 0.04757525399327278
            },
            "localRotation": {
                "x": -0.7071067094802856,
                "y": 5.3024518764418378e-33,
                "z": 5.302451141757868e-33,
                "w": 0.7071068286895752
            }
        },
        {
            "localPosition": {
                "x": -0.022079385817050935,
                "y": -0.010925745591521263,
                "z": 0.008982541039586068
            },
            "localRotation": {
                "x": -0.5323036313056946,
                "y": -0.14131203293800355,
                "z": 0.2141653299331665,
                "w": 0.8067323565483093
            }
        },
...and onwards...
````

4. Every locomtion cycle also gets **mirrored poses (left and right bones switched)** to allow the crossing of arms and legs while running.

5. The "boneMovement" script looks at this JSON file and restores it to an array of this information and can **transform each bone with the recorded values to match the keyframe.**

6. Each locomotion cycle will switch between pass and reach and then the mirrored pass and reach, **interpolating between the positions by a curve.** It also interpolates between walking and running by the velocity, and returns to a standing position when velocity is 0.

### In short, by creating only 5 keyframes in Blender:
1. Walk Pass
2. Walk Reach
3. Run Pass
4. Run Reach
5. Standing

I can get a **fully functional locomotion animation system without touching the animator inside modeling tools.**

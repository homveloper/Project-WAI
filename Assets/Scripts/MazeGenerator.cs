using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WallState{
    // 0000 is NO WALLS
    // 1111 is LEFT,RIGHT,UP,DOWN

    LEFT = 8,   // 1000
    RIGHT = 4,  // 0100
    UP = 2,     // 0010
    DOWN = 1    // 0001
}



public class MazeGenerator
{
    public static WallState[,] generate(int width, int height){

        WallState[,] maze = new WallState[width,height];

        WallState inititial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        for(int i =0; i<width; i++){
            for(int j=0; j<height; j++){
                maze[i,j] = inititial;
            }
        }

        return maze;
    }
}

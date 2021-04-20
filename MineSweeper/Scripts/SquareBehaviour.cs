using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehaviour : MonoBehaviour {
    public int meta, index;

    public bool revealed = false, flagged = false;

    public int[] neighbors = new int[8]{-1,-1,-1,-1,-1,-1,-1,-1};
    private void OnMouseOver() {
        BoardManager bm = GameObject.Find("BoardManager").GetComponent<BoardManager>();
        if (Input.GetMouseButtonDown(0) && !flagged && !revealed) {
            bm.RevealTile(this);
        }
        else if (Input.GetMouseButtonDown(0) && !flagged) {
            bm.RevealStack(this);
        }
        else if (Input.GetMouseButtonDown(1) && !revealed) {
            bm.FlagTile(this);
        }
        if (bm.autoClick && !flagged) {
            bm.RevealTile(this);
        }

    }

    internal void DetermineNeighbors(int width, int height) {

        bool left, right, top, bottom;
        left = index % width == 0;
        right = (index + 1) % width == 0;
        bottom = (index >= 0) && (index < width);
        top = index >= width * (height - 1);

        if (!bottom) {
            if (!left) neighbors[0] = index - width - 1;
            neighbors[1] = index - width;
            if (!right) neighbors[2] = index - width + 1;
        }
        if (!left) neighbors[3] = index - 1;
        if(!right) neighbors[4] = index +1;
        if (!top) {
            if (!left) neighbors[5] = index + width - 1;
            neighbors[6] = index + width;
            if (!right) neighbors[7] = index + width + 1;
        }
    }
}

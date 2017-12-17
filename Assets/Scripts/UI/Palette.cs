using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Palette {

    public static Color LandMin = rgb(79, 141, 47);
    public static Color LandMax = rgb(58, 124, 37);
    public static Color LandEdge = rgb(211, 187, 103);
    public static Color Water = rgb(52, 152, 219);
    public static Color Chartreuse = rgb(216, 236, 47);
    public static Color Sand = rgb(246, 215, 176);
    public static Color Coast = rgb(2, 255, 236);
    public static Color Alizarin = rgb(231, 76, 60);
    public static Color PeterRiver = rgb(52, 152, 219);
    public static Color Amethyst = rgb(155, 89, 182);
    public static Color Carrot = rgb(230, 126, 34);
    public static Color FadeValid = rgb(26, 188, 156, 150);
    public static Color FadeWarning = rgb(231, 154, 60, 150);
    public static Color FadeInvalid = rgb(231, 76, 60, 150);
    public static Color WhiteFade = rgb(255, 255, 255, 155);
    public static Color YellowFade = rgb(255, 255, 0, 155);
    public static Color GrainStart = rgb(96, 68, 26);
    public static Color GrainEnd = rgb(39, 174, 96);
    public static Color GrainReady = rgb(241, 196, 15);

    public static Color rgb(float r, float g, float b, float a = 255f) {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}

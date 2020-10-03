using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Color Palette", menuName = "Color Palette")]

public class ColorPalettte : ScriptableObject
{
    // Start is called before the first frame update

    [SerializeField]
    public List<Color> colors;

    public List<Color> sColors => colors;
}

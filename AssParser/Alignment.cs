#pragma warning disable CS1591
namespace AssParser;

/// <summary>
/// ASS: After the layout of the numpad (1-3 sub, 4-6 mid, 7-9 top)<br/>
/// SSA: (1-3 sub, 4-6 top, 7-9 mid)
/// </summary>
public enum Alignment
{
    SubLeft = 1,
    SubCenter,
    SubRight,
    MidLeft,
    MidCenter,
    MidRight,
    TopLeft,
    TopCenter,
    TopRight,
    // ReSharper disable InconsistentNaming
    SubLeftSSA = SubLeft,
    SubCenterSSA = SubCenter,
    SubRightSSA = SubRight,
    MidLeftSSA = TopLeft,
    MidCenterSSA = TopCenter,
    MidRightSSA = TopRight,
    TopLeftSSA = MidLeft,
    TopCenterSSA = MidCenter,
    TopRightSSA = MidRight
    // ReSharper restore InconsistentNaming
}

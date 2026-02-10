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

public static class AlignmentHelper
{
    extension(Alignment alignment)
    {
        public bool IsLeft => alignment is Alignment.SubLeft or Alignment.MidLeft or Alignment.TopLeft;

        public bool IsCenter => alignment is Alignment.SubCenter or Alignment.MidCenter or Alignment.TopCenter;

        public bool IsRight => alignment is Alignment.SubRight or Alignment.MidRight or Alignment.TopRight;

        public bool IsSub => alignment is Alignment.SubLeft or Alignment.SubCenter or Alignment.SubRight;

        public bool IsMid => alignment is Alignment.MidLeft or Alignment.MidCenter or Alignment.MidRight;

        public bool IsTop => alignment is Alignment.TopLeft or Alignment.TopCenter or Alignment.TopRight;
    }
}

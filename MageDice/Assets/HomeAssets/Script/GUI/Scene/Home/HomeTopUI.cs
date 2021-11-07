using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTopUI : MonoSingleton<HomeTopUI>
{
    [Header("Linker")]
    public AvatarIcon avatarIcon;
    public BoosterTopUI cupBoosterUI;
    public BoosterTopUI coinBoosterUI;
    public BoosterTopUI cashBoosterUI;
}

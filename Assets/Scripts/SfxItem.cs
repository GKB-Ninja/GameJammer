using UnityEngine;

[CreateAssetMenu(menuName = "Sfx/Item Sfx")]
public class SfxItem : ScriptableObject
{
    public AudioClip Grab;
    public AudioClip Drag;
    public AudioClip Drop;
    public AudioClip Intervene;
    public AudioClip ReturnBack;

    private AudioSource src = new AudioSource();

    
    public void Play(ItemSfx sfx)
    {
        switch (sfx)
        {
            case ItemSfx.Grab:
                src.clip = Grab;
                src.Play();
                break;

            case ItemSfx.Drag:
                src.clip = Drag;
                src.Play();
                break;

            case ItemSfx.Drop:
                src.clip = Drop;
                src.Play();
                break;

            case ItemSfx.Intervene:
                src.clip = Intervene;
                src.Play();
                break;

            case ItemSfx.ReturnBack:
                src.clip = ReturnBack;
                src.Play();
                break;
        }
    }
}
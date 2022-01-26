using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource BGM;
    public AudioClip[] clips;
    
    public TextMeshProUGUI trackName;
    public Image musicBg;
    Dictionary<AudioClip, bool> clipsD =
    new Dictionary<AudioClip, bool>();
    void Start()
    {
        foreach (var clip in clips)
        {
            clipsD.Add(clip , false);
        }
        playNext();
    }

    // Update is called once per frame
    void Update()
    {
        if(BGM.time == BGM.clip.length  ){
            playNext();
        }
        if(BGM.time == 0 && !BGM.isPlaying){
            playNext();
        }
    }
    void loadImg(){
        Sprite tmp = Resources.Load<Sprite>(BGM.clip.name);
        if(tmp==null)
            musicBg.sprite = Resources.Load<Sprite>("noImage");
        else
        musicBg.sprite = tmp;
        
        trackName.text = "Now playing: " + BGM.clip.name;
    }
    public void playNext(){
        BGM.Stop();
        foreach (KeyValuePair<AudioClip, bool> kvp in clipsD)
            {
                if(!kvp.Value){
                    BGM.clip = kvp.Key;
                    BGM.Play();
                    clipsD[kvp.Key] = true;
                    loadImg();
                    return;
                }
            }
            //reset if all true
            foreach (AudioClip c in clips)
            {
                clipsD[c] = false;
            }
            
    }
}

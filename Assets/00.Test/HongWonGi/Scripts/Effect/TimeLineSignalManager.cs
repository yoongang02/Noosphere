using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeLineSignalManager : MonoBehaviour
{
    private Animator _player;
    [SerializeField] private Animator _npc;
    [SerializeField] private Animator _friendNpc;
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Animator>();
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _cameraData = _mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        }
    }

    public void StartPlayerAnim(string playerAnim)
   {
       _player.SetBool(playerAnim,true);
   }

   public void EndPlayerAnim(string playerAnim)
   {
       _player.SetBool(playerAnim,false);
   }

   public void StartNpcAnim(string npcAnim)
   {
       _npc.SetBool(npcAnim,true);
   }

   public void EndNpcAnim(string npcAnim)
   {
       _npc.SetBool(npcAnim,false);
   }
   public void StartFriendNpcAnim(string npcAnim)
   {
       _friendNpc.SetBool(npcAnim,true);
   }

   public void EndFriendNpcAnim(string npcAnim)
   {
       _friendNpc.SetBool(npcAnim,false);
   }
   
   /// <summary>
   /// ///////////////////////////////////////////////////
   /// </summary>
   [Header("정신세계 이펙트")]
   [SerializeField] private RawImage _vhsImage;
   [SerializeField] private Volume _vhsVolume;
   [SerializeField] private GameObject _vhsObj;
   [SerializeField] private float _duration = 5f;
   private Camera _mainCamera;
   private UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _cameraData;
   private async UniTaskVoid StartAutoEffect()
   {
       _vhsObj.SetActive(true);
       _cameraData.renderPostProcessing = true;

       float elapsedTime = 0f;
       
       // 5초동안 게이지 증가
       while (elapsedTime < _duration)
       {
           elapsedTime += Time.deltaTime;
           float value = elapsedTime / _duration;
           
           Color color = _vhsImage.color;
           color.a = value;
           _vhsImage.color = color;
           _vhsVolume.weight = value;
           
           await UniTask.Yield();
       }

       _vhsObj.SetActive(false);
       _cameraData.renderPostProcessing = false;
       DontDestroyOnLoad(_player.gameObject);
   }
   public void OnStartMentalProcess()
   {
       StartAutoEffect().Forget();
   }

   public void OnLoadScene(string SceneName)
   {
       SceneManager.LoadSceneAsync(SceneName);
   }

}

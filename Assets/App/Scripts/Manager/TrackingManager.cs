using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TrackingManager : AutoGenerateBaseManager<TrackingManager>
{
    private readonly static string TrackerAseetAddress = "Assets/App/Prefabs/Tracker.prefab";

    public Camera PointingCamera { get; private set; } = null;
    public MotionUpdater MainMotionUpdater { get { return mainMotionUpdater; } }
    public TrackingState MainTrackingState { get; private set; } = null;
    public KinectState[] KinectStates { get { return kinectStates; } }
    public Vector3 BaseTackingPos { get { return baseTrackingPos; } }
    public bool IsRunning { get; private set; } = false;
    public bool IsLatestUpdate { get; private set; } = false;
    public bool IsExistBody { get { return lastFrameData.NumOfBodies > 0; } }
    public int ConnectedKinectCount { get; private set; }

    public BackgroundData lastFrameData = new BackgroundData();

    [SerializeField]
    private DebugUI debugUI = null;
    [SerializeField]
    private PointerUI pointerUI = null;
    [SerializeField]
    private MotionUpdater mainMotionUpdater = null;
    [SerializeField]
    private Vector3 baseTrackingPos = new Vector3(0, 0, 2.5f);
    [SerializeField]
    private int maxTrackingCount = 8;

    private List<int> beforeTrackBodyIdList   = new List<int>();
    private List<int> trackBodyIdList         = new List<int>();
    private List<int> addedTrackBodyIdList    = new List<int>();
    private List<int> removedTrackBodyIdList  = new List<int>();

    private KinectState[] kinectStates = null;
    private TrackingState[] trackingStates = null;

    /// <summary>
    /// �}�l�[�W���[�𐶐����� (�Q�[�����[�h���Ɏ����ŌĂ΂��)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void GenerateThisManager()
    {
        GenerateManager();
    }

    private void Awake()
    {
        GenerateKinectStates();

        trackingStates = new TrackingState[maxTrackingCount];

        Addressables.LoadAssetAsync<GameObject>(TrackerAseetAddress).Completed += InstantiateTrakers;
    }

    private void Update()
    {
        UpdatePointingCamera();
        UpdateTracking();

        if (pointerUI != null)
        {
            pointerUI.UpdateDebugPointer(MainTrackingState);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ToggleEnableDebugUI();
        }
    }

    private void OnApplicationQuit()
    {
        DisposeTrackingProviders();
    }

    /// <summary>
    /// Addressable�œǂݍ��܂ꂽ�I�u�W�F�N�g����g���b�J�[�̃C���X�^���X�𐶐�����
    /// </summary>
    /// <param name="handle"></param>
    private void InstantiateTrakers(AsyncOperationHandle<GameObject> handle)
    {
        for (int i = 0; i < trackingStates.Length; i++)
        {
            trackingStates[i] = Instantiate(handle.Result, transform).GetComponent<TrackingState>();
            trackingStates[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// TrackingProvider�𐶐�����
    /// </summary>
    private void GenerateKinectStates()
    {
        // Kinect���q�����Ă��鐔���擾����
        int connectedCount = Device.GetInstalledCount();
        ConnectedKinectCount = connectedCount;

        kinectStates = new KinectState[connectedCount];
        for(int i = 0; i < kinectStates.Length; i++)
        {
            kinectStates[i] = new KinectState(i);
        }
    }

    /// <summary>
    /// TrackingProvider��j������
    /// </summary>
    private void DisposeTrackingProviders()
    {
        foreach(KinectState kinectState in kinectStates)
        {
            kinectState.Dispose();
        }
    }

    /// <summary>
    /// �ǂꂩ��ł�Kinect���A�N�e�B�u��
    /// </summary>
    /// <returns></returns>
    private bool IsAnyKinectRunning()
    {
        bool isRunning = false;

        foreach (KinectState kinectState in kinectStates)
        {
            isRunning = kinectState.IsRunning;
            if (isRunning) break;
        }

        return isRunning;
    }

    /// <summary>
    /// �S�Ă�Kinect���ŐV�̃t���[���f�[�^�������Ă��邩
    /// </summary>
    /// <returns></returns>
    private bool IsLatestUpdatedOfAllKinect()
    {
        bool isLatestUpdate = true;

        foreach (KinectState kinectState in kinectStates)
        {
            isLatestUpdate = kinectState.IsLatestUpdate;
            if (!isLatestUpdate) break;
        }

        return isLatestUpdate;
    }

    private void UpdateTracking()
    {
        IsRunning = IsAnyKinectRunning();

        foreach(KinectState kinectState in kinectStates)
        {
            kinectState.UpdateState();
            lastFrameData = kinectState.LastFrameData;
        }

        if (trackingStates == null) return;

        IsLatestUpdate = IsLatestUpdatedOfAllKinect();

        if (IsLatestUpdate)
        {
            UpdateTrackerActivate();
            // ���C���g���b�J�[�݂̂ɔ������郂�[�V�������m���X�V����
            if (MainTrackingState != null)
            {
                mainMotionUpdater.UpdateMotionDetectersByTracker(MainTrackingState);
            }
        }

        foreach(TrackingState state in trackingStates)
        {
            if (state == null) continue;
            if (!state.gameObject.activeSelf) continue;

            state.UpdateState(lastFrameData);
        }
    }

    /// <summary>
    /// �g���b�J�[�̗L�������X�V����
    /// </summary>
    private void UpdateTrackerActivate()
    {
        int[] bodyIds = lastFrameData.GetBodyIds();

        // �e��Body���X�g���X�V����
        beforeTrackBodyIdList.Clear();
        beforeTrackBodyIdList.AddRange(trackBodyIdList);
        trackBodyIdList.Clear();
        trackBodyIdList.AddRange(bodyIds);
        addedTrackBodyIdList = trackBodyIdList.Except(beforeTrackBodyIdList).ToList();
        removedTrackBodyIdList = beforeTrackBodyIdList.Except(trackBodyIdList).ToList();

        // �g���b�L���O����Ȃ��Ȃ���State�͖����ɂ���
        List<TrackingState> removedStateList = trackingStates.Where(state => removedTrackBodyIdList.Contains(state.BodyId)).ToList();
        removedStateList.ForEach((state) => 
        {
            state.gameObject.SetActive(false);
            if (MainTrackingState == state)
            {
                TrackingState nextMainState = trackingStates.FirstOrDefault(state => state.gameObject.activeSelf);
                MainTrackingState = nextMainState;

                nextMainState?.tracker.SetBoneMaterial(true);
            }
        });

        // �V���Ƀg���b�L���O���ꂽId��State�Ɋ���U��
        foreach(int addedBodyId in addedTrackBodyIdList)
        {
            TrackingState addedState = trackingStates.FirstOrDefault(state => !state.gameObject.activeSelf);
            // �����ɂȂ��Ă���X�e�[�g���Ȃ��ꍇ�́A����Id���g���b�L���O����Ȃ��������Ƃɂ���
            if (addedState == null)
            {
                trackBodyIdList.Remove(addedBodyId);
                continue;
            }

            addedState.gameObject.SetActive(true);
            addedState.BodyId = addedBodyId;

            bool isNoneMainState = MainTrackingState == null;
            if (isNoneMainState)
            {
                MainTrackingState = addedState;
            }
            // ���C���ǐՒ��̃X�e�[�g�̓{�[���̐F��ς���
            addedState.tracker.SetBoneMaterial(isNoneMainState);
        }
    }

    private void UpdatePointingCamera()
    {
        if (PointingCamera != null) return;

        PointingCamera = Camera.main;
    }

    /// <summary>
    /// �f�o�b�O�pUI�̕\���A��\����؂�ւ���
    /// </summary>
    public void ToggleEnableDebugUI()
    {
        GameObject debugUIObj = debugUI.gameObject;
        debugUIObj.SetActive(!debugUIObj.activeSelf);
    }

    /// <summary>
    /// �g���b�L���O�Ώۂ��g���b�L���O���_
    /// </summary>
    /// <returns></returns>
    public float? GetDistance2DToBaseByTracker()
    {
        if (MainTrackingState == null) return null;

        return TransformUtil.Calc2DDistance(baseTrackingPos, MainTrackingState.GetRootPos());
    }

    /// <summary>
    /// �S�Ẵ��[�V�������m�̃C�x���g��S�ď���
    /// </summary>
    public void ClearAllMotionAction()
    {
        mainMotionUpdater.ClearAllMotionDetectedAction();

        foreach (TrackingState state in trackingStates)
        {
            if (state == null) continue;

            state.MotionUpdater.ClearAllMotionDetectedAction();
        }
    }
}

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTE : MonoBehaviour
{
    public event Action OnQTEStart;
    public event Action<IQTECaller> OnQTEEnd;
    public event Action<int> OnQTERoundFailed;

    public static QTE instance;
    public IQTECaller caller;

    public enum QTE_TYPE
    {
        Reaction,
        SmashingButtons,
    }

    private QTE_TYPE type;

    [SerializeField]
    private GameObject qteParent;

    [SerializeField]
    private Image[] inputKeyImageArray; // W A S D
    private Color originalKeyImageColor;
    [SerializeField]
    private Image background;
   

    private Dictionary<int, Vector2> keyToVectorDict;
    [SerializeField]
    private bool qtePlaying = false;

    public bool IsQTEPlaying
    {
        get { return qtePlaying; }
    }


    private int randomKeyIndex = 0;

    private readonly string KEY_ANIMATION = "IS_KEY_ANIMATION";

    private int qteRoundsFinished = 0;
    private int qteRounds = 6;

    private float qteRoundTimer;
    private float qteRoundTimerTotal = 0.6f;

    private int qteFailedDamage = 1;


    private int buttonPressesToFinishSmashingQTE = 30;
    private int buttonPresses = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        qteParent.SetActive(false);

        originalKeyImageColor = inputKeyImageArray[0].color;
        keyToVectorDict = new Dictionary<int, Vector2>() {
            {0, new Vector2(0,1) }, 
            {1, new Vector2(-1,0)},
            {2, new Vector2(0,-1)},
            {3, new Vector2(1,0)},
        };
    }

    public void StartQTE(IQTECaller caller, QTE_TYPE type)
    {
        if (!qtePlaying)
        {
            this.type = type;
            this.caller = caller;
            qtePlaying = true;
            qteParent.SetActive(true);

            if (type == QTE_TYPE.Reaction)
            {
                randomKeyIndex = GetRandomIndex();
                SetQTEButtonActive(randomKeyIndex);
                qteRoundsFinished = 0;
                OnQTEStart?.Invoke();
            }
            if(type == QTE_TYPE.SmashingButtons)
            {
                buttonPresses = 0;

                randomKeyIndex = GetNextSmashButtonIndex();
                SetQTEButtonActive(randomKeyIndex);

                OnQTEStart?.Invoke();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(qtePlaying)
        {
            Vector2 input = InputManager.Instance.GetQTEActions();

            if (type == QTE_TYPE.Reaction)
            {
                qteRoundTimer -= Time.deltaTime;
                //if run out of time, get damaged, get to next round
                if (qteRoundTimer < 0)
                {
                    StartNextQTERound();
                    OnQTERoundFailed?.Invoke(qteFailedDamage);
                }
                //if press key in time, get to next round
                if(input != Vector2.zero)
                {
                    if (keyToVectorDict[randomKeyIndex] == input)
                    {
                         StartNextQTERound();
                    }
                    else
                    {
                         StartNextQTERound();
                         OnQTERoundFailed?.Invoke(qteFailedDamage);
                    }
                }
            }
            if (type == QTE_TYPE.SmashingButtons)
            {
                if(keyToVectorDict[randomKeyIndex] == input)
                {
                    RefreshQTE(randomKeyIndex);
                    randomKeyIndex = GetNextSmashButtonIndex();
                    SetQTEButtonActive(randomKeyIndex);
                    buttonPresses++;
                }
                if(buttonPresses >= buttonPressesToFinishSmashingQTE)
                {
                    EndQTE();
                }
            }
        }
    }

    private void StartQTERound()
    {
        randomKeyIndex = GetRandomIndex();
        SetQTEButtonActive(randomKeyIndex);
        RestartRoundTimer();
    }

    public void EndQTE()
    {
        qteRoundsFinished = 0;
        qtePlaying = false;
        qteParent.SetActive(false);
        OnQTEEnd?.Invoke(caller);
        RefreshQTE(randomKeyIndex);
    }

    private void RestartRoundTimer()
    {
        qteRoundTimer = qteRoundTimerTotal;
    }

    private int GetRandomIndex()
    {
        int index = 0;
        do
        {
            index = UnityEngine.Random.Range(0, inputKeyImageArray.Length);
        } while (index == randomKeyIndex);
        return index;
    }

    private void SetQTEButtonActive(int index)
    {
        inputKeyImageArray[index].GetComponent<Animator>().SetBool(KEY_ANIMATION, true);
        RestartRoundTimer();
    }
    private void RefreshQTE(int index)
    {
        inputKeyImageArray[index].GetComponent<Animator>().SetBool(KEY_ANIMATION, false);
        inputKeyImageArray[index].color = originalKeyImageColor;
    }

    private void StartNextQTERound()
    {
        RefreshQTE(randomKeyIndex);
        if (qteRoundsFinished < qteRounds)
        {
            StartQTERound();
            qteRoundsFinished++;
            RestartRoundTimer();
        }
        else
        {
            EndQTE();
        }
    }


    private int GetNextSmashButtonIndex()
    {
        if(buttonPresses % 2 == 0)
        {
            //A
            return 1;
        }
        else
        {
            //D
            return 3;
        }
    }
}

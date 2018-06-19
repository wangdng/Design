using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillCtrl : UIBase
{
    enum SkillType
    {
        BigSkill,
        SmallSkill,
        SpecialSkill
    }

    private AnimEventManager playerAnimEventMnager;

    UIBehaviour skill_big;
    UIBehaviour skill_big_cdmask;

    UIBehaviour skill_small;
    UIBehaviour skill_small_cdmask;

    UIBehaviour skill_special;
    UIBehaviour skill_special_cdmask;

    UIBehaviour blood;

    Image bloodImage;


    public override void ProcessEvent(MsgBase msgbase)
    {
        switch (msgbase.MsgId)
        {
            case (ushort)UIEvent.BloodLose:
                {
                    MsgFloat tmpMsg = (MsgFloat)msgbase;

                    bloodImage.fillAmount = Mathf.Clamp01(tmpMsg.GetValue());

                    if (bloodImage.fillAmount <= 0)
                    {
                        MsgBase deadMsg = new MsgBase((ushort)CharactorDataEvent.MonsterPlayerDead);

                        SendMessage(deadMsg);
                    }
                    break;
                }
        }
    }

    List<Action> CreateSkillEvents(SkillType type)
    {
        List<Action> events = new List<Action>();

        switch (type)
        {
            case SkillType.BigSkill:
                {
                    Action BigSkillDo = () =>
                    {
                        SkillCD bigSkillCD = skill_big_cdmask.GetComponent<SkillCD>();

                        if (!bigSkillCD.isInCD)
                        {
                            tmpMsg.ChangeMsgId((ushort)CharactorAnimEvent.BigAttack);

                            SendMessage(tmpMsg);

                            bigSkillCD.StartCD();
                        }
                    };

                    events.Add(BigSkillDo);

                    Action BigSkillEnd = () =>
                    {
                        tmpMsg.ChangeMsgId((ushort)CharactorAnimEvent.Idle);
                        SendMessage(tmpMsg);
                    };

                    events.Add(BigSkillEnd);
                }
                break;
            case SkillType.SmallSkill:
                {
                    Action SmallSkillDo = () =>
                    {
                        SkillCD smallSkillCD = skill_small_cdmask.GetComponent<SkillCD>();

                        if (!smallSkillCD.isInCD)
                        {
                            tmpMsg.ChangeMsgId((ushort)CharactorAnimEvent.SmallAttack);

                            SendMessage(tmpMsg);

                            smallSkillCD.StartCD();
                        }
                    };

                    events.Add(SmallSkillDo);

                    Action SmallSkillEnd = () =>
                    {
                        tmpMsg.ChangeMsgId((ushort)CharactorAnimEvent.Idle);
                        SendMessage(tmpMsg);
                    };

                    events.Add(SmallSkillEnd);
                }
                break;
            case SkillType.SpecialSkill:
                {
                    Action SpecailSkillDo = () =>
                    {
                        SkillCD specialSkillCD = skill_special_cdmask.GetComponent<SkillCD>();

                        if (!specialSkillCD.isInCD)
                        {
                            tmpMsg.ChangeMsgId((ushort)CharactorAnimEvent.SpecialAttack);

                            SendMessage(tmpMsg);

                            specialSkillCD.StartCD();
                        }
                    };

                    events.Add(SpecailSkillDo);

                    Action SpeciallSkillEnd = () =>
                    {
                        tmpMsg.ChangeMsgId((ushort)CharactorAnimEvent.Idle);
                        SendMessage(tmpMsg);
                    };

                    events.Add(SpeciallSkillEnd);
                }
                break;
        }

        return events;
    }

    void SkillBigClick()
    {
        RegisterSkillEvent(CreateSkillEvents(SkillType.BigSkill));
    }

    void SkillSmallClick()
    {
        RegisterSkillEvent(CreateSkillEvents(SkillType.SmallSkill));
    }

    void SkillSpecailClick()
    {
        RegisterSkillEvent(CreateSkillEvents(SkillType.SpecialSkill));
    }

    void RegisterSkillEvent(List<Action> events)
    {
        Dictionary<string, AnimEventBase> tmpEvents = CreateSkillEvent(events);

        playerAnimEventMnager.RegisterEvents(tmpEvents);
    }

    Dictionary<string, AnimEventBase> CreateSkillEvent(List<Action> events)
    {
        PlayerSkillEvent skillEvent = new PlayerSkillEvent();

        skillEvent.Init(0, 0, events[0]);

        PlayerSkillEndEvent skillEnd = new PlayerSkillEndEvent();

        skillEnd.Init(0, 1.5f, events[1]);

        Dictionary<string, AnimEventBase> tmpEvents = new Dictionary<string, AnimEventBase>();
        tmpEvents.Add("skillEvent", skillEvent);
        tmpEvents.Add("skillEnd", skillEnd);

        return tmpEvents;
    }

    private void LateUpdate()
    {
        playerAnimEventMnager.OnUpdate();
    }

    private MsgBase tmpMsg;

    private void Awake()
    {
        msgIds = new ushort[]
        {
            (ushort)UIEvent.BloodLose,
        };

        RegisterSelf(this, msgIds);

    }

    private void Start()
    {
        skill_big = UIManager.instance.GetGameObject(this, "Skill_big").GetComponent<UIBehaviour>();

        skill_big_cdmask = UIManager.instance.GetGameObject(this, "skillBigCD").GetComponent<UIBehaviour>();

        skill_small = UIManager.instance.GetGameObject(this, "Skill_Small").GetComponent<UIBehaviour>();

        skill_small_cdmask = UIManager.instance.GetGameObject(this, "skillSmallCD").GetComponent<UIBehaviour>();

        skill_special = UIManager.instance.GetGameObject(this, "Skill_Special").GetComponent<UIBehaviour>();

        skill_special_cdmask = UIManager.instance.GetGameObject(this, "skillSpecialCD").GetComponent<UIBehaviour>();

        blood = UIManager.instance.GetGameObject(this, "Blood").GetComponent<UIBehaviour>();

        bloodImage = blood.GetComponent<Image>();

        skill_big.AddButtonListener(SkillBigClick);
        skill_small.AddButtonListener(SkillSmallClick);
        skill_special.AddButtonListener(SkillSpecailClick);

        tmpMsg = new MsgBase();

        playerAnimEventMnager = new AnimEventManager();
    }
}

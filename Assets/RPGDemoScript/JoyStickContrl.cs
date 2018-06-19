using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using LuaInterface;

using UnityEngine.UI;

public class JoyStickContrl : UIBase
{
    Text h;
    Text v;

    ETCJoystick Ej;

    JoyStickMsg msg;

    MsgBase AttackMsg;

    private AnimEventManager playerAnimEventMnager;

    public override void ProcessEvent(MsgBase msgbase)
    {

    }

    private void Awake()
    {
        msgIds = new ushort[]
            { };

        RegisterSelf(this, msgIds);
    }

    public void JoyStickMoveBegin()
    {
       
    }

    public void JoyStickMove(Vector2 data)
    {
        h.text = data.x.ToString();
        v.text = data.y.ToString();

        msg.ChangeMsgId((ushort)CharactorDataEvent.joyStick);
        msg.ChangejoyStickData(data);
        SendMessage(msg);
    }

    public void JoyStickEnd()
    {
        msg.ChangeMsgId((ushort)CharactorDataEvent.joyStickEnd);

        SendMessage(msg);
    }

    private void LateUpdate()
    {
        playerAnimEventMnager.OnUpdate();
    }

    void OnAttackBtnUp()
    {
        Dictionary<string, AnimEventBase> attackEvents = CreateAttackEvent();
        playerAnimEventMnager.RegisterEvents(attackEvents);
    }

    Dictionary<string, AnimEventBase> CreateAttackEvent()
    {
        PlayerAttackEvent attackEvent = new PlayerAttackEvent();
        attackEvent.Init(0, 0, OnAttackEvent);

        PlayerAttackEndEvent attackendEvent = new PlayerAttackEndEvent();
        attackendEvent.Init(0, 1, OnAttackEndEvent);

        Dictionary<string, AnimEventBase> tmpEvents = new Dictionary<string, AnimEventBase>();
        tmpEvents.Add("Attack", attackEvent);
        tmpEvents.Add("AttackEnd", attackendEvent);

        return tmpEvents;
    }

    void OnAttackEndEvent()
    {
        AttackMsg.ChangeMsgId((ushort)CharactorAnimEvent.Idle);

        SendMessage(AttackMsg);
    }

    void OnAttackEvent()
    {
        AttackMsg.ChangeMsgId((ushort)CharactorAnimEvent.Attack);

        SendMessage(AttackMsg);
    }

    void Start()
    {
         h = UIManager.instance.GetGameObject(this, "Horizontal").GetComponent<Text>();

         v = UIManager.instance.GetGameObject(this, "Vertical").GetComponent<Text>();

        playerAnimEventMnager = new AnimEventManager();

        Ej = this.GetComponent<ETCJoystick>();

        Ej.onMoveStart.AddListener(JoyStickMoveBegin);

        Ej.onMove.AddListener(JoyStickMove);

        Ej.onMoveEnd.AddListener(JoyStickEnd);

        ETCButton attackBtn = UIManager.instance.GetGameObject(this, "Attack").GetComponent<ETCButton>();

        attackBtn.onUp.AddListener(OnAttackBtnUp);

        msg = new JoyStickMsg();

        AttackMsg = new MsgBase();
    }

    private void OnDestroy()
    {
         h = null;
         v = null;

         msg = null;
        AttackMsg = null;
    }
}

﻿
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Shared;
using Shared.DataModel;
using Shared.Protocol;
using Server.UModule;

using Server.GModule;
using MessagePack;
using Shared.Protocol.Message;
using System;

namespace Server
{

    [RefType(typeof(AccountService))]
    public partial class AccountServiceRef : ActorRef
    {
        public void rpc_create_account(String username, String password, String extra, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
            if (this.fromActor.HostId == toHostId)
            {
                ((AccountService)Host.Instance.GetActor(this.toActorId)).CreateAccount(username, password, extra, callback);
                return;
            }
            var msg = new CreateAccountReq()
            {
                username=username,
                password=password,
                extra=extra
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<CreateAccountReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.CREATE_ACCOUNT_REQ, msg, cb);
        }

        public void rpc_delete_account(String username, String password, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
            if (this.fromActor.HostId == toHostId)
            {
                ((AccountService)Host.Instance.GetActor(this.toActorId)).DeleteAccount(username, password, callback);
                return;
            }
            var msg = new DeleteAccountReq()
            {
                username=username,
                password=password
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<DeleteAccountReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.DELETE_ACCOUNT_REQ, msg, cb);
        }

        public void rpc_login(String username, String password, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
            if (this.fromActor.HostId == toHostId)
            {
                ((AccountService)Host.Instance.GetActor(this.toActorId)).Login(username, password, callback);
                return;
            }
            var msg = new LoginReq()
            {
                username=username,
                password=password
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<LoginReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.LOGIN_REQ, msg, cb);
        }

        public void rpc_reset_password(String username, String email)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
           if (this.fromActor.HostId == toHostId)
           {
               ((AccountService)Host.Instance.GetActor(this.toActorId)).ResetPassword(username, email);
               return;
           }
           var msg = new ResetPasswordReq()
           {
                username=username,
                email=email
           };
           this.CallRemoteMethod(ProtocolCode.RESET_PASSWORD_REQ, msg, null);
        }
    }
}


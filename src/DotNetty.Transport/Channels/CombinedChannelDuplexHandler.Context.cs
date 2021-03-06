﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Transport.Channels
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Common.Concurrency;
    using DotNetty.Common.Utilities;

    partial class CombinedChannelDuplexHandler<TIn, TOut>
    {
        sealed class DelegatingChannelHandlerContext : IChannelHandlerContext
        {
            private static readonly Action<object> s_removeAction = OnRemove;

            private readonly IChannelHandlerContext _ctx;
            private readonly IChannelHandler _handler;
            private readonly Action<Exception> _onError;
            private bool _removed;

            public DelegatingChannelHandlerContext(IChannelHandlerContext ctx, IChannelHandler handler, Action<Exception> onError = null)
            {
                _ctx = ctx;
                _handler = handler;
                _onError = onError;
            }

            public IChannelHandlerContext InnerContext => _ctx;

            public IChannel Channel => _ctx.Channel;

            public IChannelPipeline Pipeline => _ctx.Pipeline;

            public IByteBufferAllocator Allocator => _ctx.Allocator;

            public IEventExecutor Executor => _ctx.Executor;

            public string Name => _ctx.Name;

            public IChannelHandler Handler => _ctx.Handler;

            public bool Removed => _removed || _ctx.Removed;

            public IChannelHandlerContext FireChannelRegistered()
            {
                _ = _ctx.FireChannelRegistered();
                return this;
            }

            public IChannelHandlerContext FireChannelUnregistered()
            {
                _ = _ctx.FireChannelUnregistered();
                return this;
            }

            public IChannelHandlerContext FireChannelActive()
            {
                _ = _ctx.FireChannelActive();
                return this;
            }

            public IChannelHandlerContext FireChannelInactive()
            {
                _ = _ctx.FireChannelInactive();
                return this;
            }

            public IChannelHandlerContext FireExceptionCaught(Exception ex)
            {
                if (_onError is object)
                {
                    _onError(ex);
                }
                else
                {
                    _ = _ctx.FireExceptionCaught(ex);
                }

                return this;
            }

            public IChannelHandlerContext FireUserEventTriggered(object evt)
            {
                _ = _ctx.FireUserEventTriggered(evt);
                return this;
            }

            public IChannelHandlerContext FireChannelRead(object message)
            {
                _ = _ctx.FireChannelRead(message);
                return this;
            }

            public IChannelHandlerContext FireChannelReadComplete()
            {
                _ = _ctx.FireChannelReadComplete();
                return this;
            }

            public IChannelHandlerContext FireChannelWritabilityChanged()
            {
                _ = _ctx.FireChannelWritabilityChanged();
                return this;
            }

            public Task BindAsync(EndPoint localAddress) => _ctx.BindAsync(localAddress);

            public Task ConnectAsync(EndPoint remoteAddress) => _ctx.ConnectAsync(remoteAddress);

            public Task ConnectAsync(EndPoint remoteAddress, EndPoint localAddress) => _ctx.ConnectAsync(remoteAddress, localAddress);

            public Task DisconnectAsync() => _ctx.DisconnectAsync();

            public Task DisconnectAsync(IPromise promise) => _ctx.DisconnectAsync(promise);

            public Task CloseAsync() => _ctx.CloseAsync();

            public Task CloseAsync(IPromise promise) => _ctx.CloseAsync(promise);

            public Task DeregisterAsync() => _ctx.DeregisterAsync();

            public Task DeregisterAsync(IPromise promise) => _ctx.DeregisterAsync(promise);

            public IChannelHandlerContext Read()
            {
                _ = _ctx.Read();
                return this;
            }

            public Task WriteAsync(object message) => _ctx.WriteAsync(message);

            public Task WriteAsync(object message, IPromise promise) => _ctx.WriteAsync(message, promise);

            public IChannelHandlerContext Flush()
            {
                _ = _ctx.Flush();
                return this;
            }

            public Task WriteAndFlushAsync(object message) => _ctx.WriteAndFlushAsync(message);

            public Task WriteAndFlushAsync(object message, IPromise promise) => _ctx.WriteAndFlushAsync(message, promise);

            public IAttribute<T> GetAttribute<T>(AttributeKey<T> key) where T : class => _ctx.GetAttribute(key);

            public bool HasAttribute<T>(AttributeKey<T> key) where T : class => _ctx.HasAttribute(key);

            public IPromise NewPromise() => _ctx.NewPromise();

            public IPromise NewPromise(object state) => _ctx.NewPromise(state);

            public IPromise VoidPromise() => _ctx.VoidPromise();

            internal void Remove()
            {
                IEventExecutor executor = Executor;
                if (executor.InEventLoop)
                {
                    Remove0();
                }
                else
                {
                    executor.Execute(s_removeAction, this);
                }
            }

            private static void OnRemove(object c)
            {
                ((DelegatingChannelHandlerContext)c).Remove0();
            }

            void Remove0()
            {
                if (_removed)
                {
                    return;
                }

                _removed = true;
                try
                {
                    _handler.HandlerRemoved(this);
                }
                catch (Exception cause)
                {
                    _ = FireExceptionCaught(
                        new ChannelPipelineException($"{StringUtil.SimpleClassName(_handler)}.handlerRemoved() has thrown an exception.", cause));
                }
            }
        }
    }
}

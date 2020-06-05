﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using QuickJS.Native;
using QuickJS.Utils;
using UnityEngine;

namespace QuickJS
{
    public class ScriptContext
    {
        public event Action<ScriptContext> OnDestroy;

        private ScriptRuntime _runtime;
        private JSContext _ctx;

        public ScriptContext(ScriptRuntime runtime)
        {
            _runtime = runtime;
            _ctx = JSApi.JS_NewContext(_runtime);
        }

        public TimerManager GetTimerManager()
        {
            return _runtime.GetTimerManager();
        }

        public ScriptRuntime GetRuntime()
        {
            return _runtime;
        }

        public bool IsContext(JSContext ctx)
        {
            return ctx.IsContext(_ctx);
        }

        public void Destroy()
        {
            try
            {
                OnDestroy?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            JSApi.JS_FreeContext(_ctx);
            _ctx = JSContext.Null;
        }

        public void AddIntrinsicOperators()
        {
            JSApi.JS_AddIntrinsicOperators(_ctx);
        }

        public void FreeValue(JSValue value)
        {
            _runtime.FreeValue(value);
        }
        
        public void FreeValues(JSValue[] values)
        {
            _runtime.FreeValues(values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSValue GetGlobalObject()
        {
            return JSApi.JS_GetGlobalObject(_ctx);
        }

        #region Builtins

        public void print_exception()
        {
            _ctx.print_exception();
        }

        [MonoPInvokeCallback(typeof(JSCFunction))]
        private static JSValue _print(JSContext ctx, JSValue this_obj, int argc, JSValue[] argv)
        {
            int i;
            var sb = new StringBuilder();
            size_t len;

            for (i = 0; i < argc; i++)
            {
                if (i != 0)
                {
                    sb.Append(' ');
                }

                var pstr = JSApi.JS_ToCStringLen(ctx, out len, argv[i]);
                if (pstr == IntPtr.Zero)
                {
                    return JSApi.JS_EXCEPTION;
                }

                var str = JSApi.GetString(pstr, len);
                if (str != null)
                {
                    sb.Append(str);
                }

                JSApi.JS_FreeCString(ctx, pstr);
            }

            sb.AppendLine();
            Debug.Log(sb.ToString());
            return JSApi.JS_UNDEFINED;
        }

        #endregion

        public void RegisterBuiltins()
        {
            var global_object = JSApi.JS_GetGlobalObject(this);

            _ctx.SetProperty(global_object, "print", _print, 1);

            JSApi.JS_FreeValue(this, global_object);
        }

        public static implicit operator JSContext(ScriptContext sc)
        {
            return sc._ctx;
        }
    }
}
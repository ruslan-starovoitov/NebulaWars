﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroFormatter;
         
namespace NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages
{
    [ZeroFormattable]
    public struct TestPositionsMessage 
    {
        [Index(0)] public Dictionary<ushort, ViewTransform>EntitiesInfo { get; set; }
        [Index(1)] public Dictionary<ushort, ushort>__RadiusInfo { get; set; }
        
        public TestPositionsMessage(Dictionary<ushort, ViewTransform>entitiesInfo,
            Dictionary<ushort, ushort> radiusInfo)
        {
            EntitiesInfo = entitiesInfo;
            __RadiusInfo = radiusInfo;
        }
    }

    [ZeroFormattable]
    public struct RadiusesMessage : ITypedMessage
    {
        [Index(0)] public Dictionary<ushort, ushort> RadiusInfo { get; set; }

        [IgnoreFormat]
        public Dictionary<ushort, float> FloatRadiusInfo =>
            RadiusInfo.ToDictionary(pair => pair.Key, pair => Mathf.HalfToFloat(pair.Value));

        public RadiusesMessage(Dictionary<ushort, ushort> radiusInfo)
        {
            RadiusInfo = radiusInfo;
        }

        public MessageType GetMessageType() => MessageType.Radiuses;
    }

    [ZeroFormattable]
    public struct DetachesMessage : ITypedMessage
    {
        [Index(0)] public ushort[] DetachedIds { get; set; }

        public DetachesMessage(ushort[] detachedIds)
        {
            DetachedIds = detachedIds;
        }

        public MessageType GetMessageType() => MessageType.Detaches;
    }

    [ZeroFormattable]
    public struct DestroysMessage : ITypedMessage
    {
        [Index(0)] public ushort[] DestroyedIds { get; set; }

        public DestroysMessage(ushort[] destroyedIds)
        {
            DestroyedIds = destroyedIds;
        }

        public MessageType GetMessageType() => MessageType.Destroys;
    }

    [ZeroFormattable]
    public struct HidesMessage : ITypedMessage
    {
        [Index(0)] public ushort[] HiddenIds { get; set; }

        public HidesMessage(ushort[] hiddenIds)
        {
            HiddenIds = hiddenIds;
        }

        public MessageType GetMessageType() => MessageType.Hides;
    }

    [ZeroFormattable]
    public struct ParentsMessage : ITypedMessage
    {
        [Index(0)] public Dictionary<ushort, ushort> ParentInfo { get; set; }

        public ParentsMessage(Dictionary<ushort, ushort> parentInfo)
        {
            ParentInfo = parentInfo;
        }

        public MessageType GetMessageType() => MessageType.Parents;
    }

    [ZeroFormattable]
    public class PositionsMessage : ITypedMessage
    {
        [Index(0)] public virtual Dictionary<ushort, ViewTransform> entitiesInfo { get; set; }

        public PositionsMessage()
        {
            
        }
        public PositionsMessage(Dictionary<ushort, ViewTransform> entitiesInfo)
        {
            this.entitiesInfo = entitiesInfo;
        }

        public MessageType GetMessageType() => MessageType.Positions;
    }

    [ZeroFormattable]
    public struct Vector2
    {
        [Index(0)] public ushort __x;
        [Index(1)] public ushort __y;
        
        [IgnoreFormat] public float X => Mathf.HalfToFloat(__x);
        [IgnoreFormat] public float Y => Mathf.HalfToFloat(__y);
        
        public Vector2(ushort x, ushort y)
        {
            __x = x;
            __y = y;
        }
        
        public Vector2(float x, float y)
        {
            __x = Mathf.FloatToHalf(x);
            __y = Mathf.FloatToHalf(y);
        }

        public override string ToString()
        {
            return $"({X}; {Y})";
        }

#if UNITY_5_3_OR_NEWER
        public static implicit operator UnityEngine.Vector2(Vector2 vector) => new UnityEngine.Vector2(vector.X, vector.Y);
        public static implicit operator Vector2(UnityEngine.Vector2 vector) => new Vector2(vector.x, vector.y);
#endif
    }
    
    /// <summary>
    /// 7 байтов
    /// </summary>
    [ZeroFormattable]
    public struct ViewTransform
    {
        [Index(0)] public ushort __x;
        [Index(1)] public ushort __z;
        [Index(2)] public ushort __angle;
        [Index(3)] public ViewTypeId typeId;

        [IgnoreFormat] public float X => Mathf.HalfToFloat(__x);
        [IgnoreFormat] public float Z => Mathf.HalfToFloat(__z);
        [IgnoreFormat] public float Angle
        {
            get => Mathf.HalfToFloat(__angle);
            set => __angle = Mathf.FloatToHalf(value);
        }

        public ViewTransform(ushort x, ushort z, ushort angle, ViewTypeId typeId)
        {
            __x = x;
            __z = z;
            __angle = angle;
            this.typeId = typeId;
        }
        
        public ViewTransform(float x, float z, float angle, ViewTypeId typeId)
        {
            __x = Mathf.FloatToHalf(x);
            __z = Mathf.FloatToHalf(z);
            __angle = Mathf.FloatToHalf(angle);
            this.typeId = typeId;
        }

        public ViewTransform(float x, float z, ViewTypeId typeId) 
            : this(x, z, 0f, typeId)
        { }

        public ViewTransform(Vector2 position, ViewTypeId typeId) 
            : this(position.X, position.Y, typeId)
        { }

        public ViewTransform(Vector2 position, float angle, ViewTypeId typeId) 
            : this(position.X, position.Y, angle, typeId)
        { }

        public Vector3 GetPosition() => new Vector3(X, 0, Z);

        public static ViewTransform operator +(ViewTransform t1, ViewTransform t2)
        {
            if(t1.typeId != t2.typeId) throw new NotSupportedException(nameof(typeId) + " не совпали!");
            return new ViewTransform(t1.X + t2.X, t1.Z + t2.Z, t1.Angle + t2.Angle, t2.typeId);
        }

        public static ViewTransform operator -(ViewTransform t1, ViewTransform t2)
        {
            if (t1.typeId != t2.typeId) throw new NotSupportedException(nameof(typeId) + " не совпали!");
            return new ViewTransform(t1.X - t2.X, t1.Z - t2.Z, t1.Angle - t2.Angle, t2.typeId);
        }

        public static ViewTransform operator *(ViewTransform t, float k)
        {
            return new ViewTransform(t.X * k, t.Z * k, t.Angle * k, t.typeId);
        }

        public static ViewTransform operator *(float k, ViewTransform t) => t * k;

        public static ViewTransform operator /(ViewTransform t, float k)
        {
            return new ViewTransform(t.X / k, t.Z / k, t.Angle / k, t.typeId);
        }
    }
    
    [ZeroFormattable]
    public class TestMessageClass1
    {
        [Index(0)] public virtual int SomeNumber { get; set; }

        public TestMessageClass1()
        {
            
        }
    }
    
    [ZeroFormattable]
    public struct TestMessageStruct1
    {
        [Index(0)] public int SomeNumber;

        public TestMessageStruct1(int someNumber)
        {
            SomeNumber = someNumber;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FrankenToilet.prideunique;

public static class SoundRandomizer
{
    private struct AudioSlot
    {
        public MemberRef Member;
        public int Index;        // element index for array or list
        public bool IsCollection;

        public AudioSlot(MemberRef member)
        {
            Member = member;
            Index = -1;
            IsCollection = false;
        }

        public AudioSlot(MemberRef member, int index)
        {
            Member = member;
            Index = index;
            IsCollection = true;
        }
    }

    private class MemberRef
    {
        public MemberInfo Member; // FieldInfo or PropertyInfo
        public UnityEngine.Object Target; // MonoBehaviour, GameObject, MonoSingleton, AudioSource, etc.

        public string Name
        {
            get
            {
                var memberName = Member?.Name ?? "<null>";
                var typeName = Target != null ? Target.GetType().Name : "<nullTarget>";
                return $"{typeName}.{memberName}";
            }
        }

        public Type MemberType
        {
            get
            {
                if (Member is FieldInfo f) return f.FieldType;
                if (Member is PropertyInfo p) return p.PropertyType;
                return null;
            }
        }


        public object GetValue()
        {
            try
            {
                if (Member is FieldInfo f)
                    return f.GetValue(Target);
                if (Member is PropertyInfo p)
                    return p.GetValue(Target, null);
            }
            catch (Exception)
            {

            }

            return null;
        }

        public void SetValue(object value)
        {
            try
            {
                if (Member is FieldInfo f)
                    f.SetValue(Target, value);
                else if (Member is PropertyInfo p)
                    p.SetValue(Target, value, null);
            }
            catch (Exception)
            {

            }
        }
    }

    private class CachedMember
    {
        public MemberInfo Member;
        public Type MemberType;
        public string Name;
        public bool IsProperty;

        public bool IsAudioSingle;
        public bool IsAudioArray;
        public bool IsAudioList;

        public bool IsGameObject;
        public bool IsGameObjectArray;
        public bool IsGameObjectList;

        public bool IsAssetReference;
        public bool IsAssetReferenceArray;
        public bool IsAssetReferenceList;

        public bool IsComponent;
        public bool IsComponentArray;
        public bool IsComponentList;

        public bool HasGetterAndSetter;
    }

    private class CachedTypeInfo
    {
        public List<CachedMember> Members = new List<CachedMember>();
    }

    private static readonly Dictionary<Type, CachedTypeInfo> s_inspectionCache = new Dictionary<Type, CachedTypeInfo>();

    public static List<AudioClip> AddressableAudioClips = new List<AudioClip>();

    public static void SwitchSounds()
    {
        var audioMembers = GetInspectorReferencedAudioMembers();

        var slots = new List<AudioSlot>();
        var values = new List<AudioClip>();

        values.AddRange(AddressableAudioClips);

        foreach (var member in audioMembers)
        {
            var type = member.MemberType;

            if (type == typeof(AudioClip))
            {
                slots.Add(new AudioSlot(member));
                values.Add(member.GetValue() as AudioClip);
            }
            else if (type == typeof(AudioClip[]))
            {
                var array = member.GetValue() as AudioClip[];
                if (array == null)
                    continue;

                for (int i = 0; i < array.Length; i++)
                {
                    slots.Add(new AudioSlot(member, i));
                    values.Add(array[i]);
                }
            }
            // List<AudioClip>
            else if (type.IsGenericType &&
                     type.GetGenericTypeDefinition() == typeof(List<>) &&
                     type.GetGenericArguments()[0] == typeof(AudioClip))
            {
                var list = member.GetValue() as List<AudioClip>;
                if (list == null)
                    continue;

                for (int i = 0; i < list.Count; i++)
                {
                    slots.Add(new AudioSlot(member, i));
                    values.Add(list[i]);
                }
            }
        }

        if (values.Count < 2)
            return;

        values.Shuffle();

        int v = 0;
        foreach (var slot in slots)
        {
            var clip = values[v++];
            if (clip == null)
                continue;

            var member = slot.Member;
            var type = member.MemberType;

            if (!slot.IsCollection)
            {
                member.SetValue(clip);
            }
            else if (type == typeof(AudioClip[]))
            {
                var array = member.GetValue() as AudioClip[];
                if (array == null || slot.Index >= array.Length)
                    continue;

                array[slot.Index] = clip;
                member.SetValue(array);
            }
            // List
            else
            {
                var list = member.GetValue() as List<AudioClip>;
                if (list == null || slot.Index >= list.Count)
                    continue;

                list[slot.Index] = clip;
                member.SetValue(list);
            }
        }
    }

    private static List<MemberRef> GetInspectorReferencedAudioMembers()
    {
        List<MemberRef> audioMembers = new List<MemberRef>();
        var seen = new HashSet<string>(); // "instanceID|memberName" to avoid duplicates

        BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        bool IsAudioClipType(Type t)
        {
            return t == typeof(AudioClip) || t == typeof(AudioClip[])
            || (t.IsGenericType &&
                t.GetGenericTypeDefinition() == typeof(List<>) &&
                t.GetGenericArguments()[0] == typeof(AudioClip)); ;
        }

        bool IsGameObjectList(Type t)
        {
            return t.IsGenericType &&
                   t.GetGenericTypeDefinition() == typeof(List<>) &&
                   t.GetGenericArguments()[0] == typeof(GameObject);
        }

        bool IsComponentArray(Type t)
        {
            return t != null && t.IsArray && typeof(UnityEngine.Component).IsAssignableFrom(t.GetElementType());
        }

        bool IsComponentList(Type t)
        {
            return t.IsGenericType &&
                   t.GetGenericTypeDefinition() == typeof(List<>) &&
                   typeof(UnityEngine.Component).IsAssignableFrom(t.GetGenericArguments()[0]);
        }

        bool IsAssetReference(Type t)
        {
            if (t == null) return false;

            if (t.FullName == "UnityEngine.AddressableAssets.AssetReference")
                return true;

            if (t.Name == "AssetReference")
                return true;

            return false;
        }

        bool IsSerializedMember(MemberInfo member)
        {

            if (member is FieldInfo f)
            {
                bool publicField = f.IsPublic;
                bool hasSerializeAttr = f.GetCustomAttributes(typeof(SerializeField), false).Length > 0;
                return publicField || hasSerializeAttr;
            }


            if (member is PropertyInfo p)
            {
                var getter = p.GetGetMethod(true);
                var setter = p.GetSetMethod(true);
                bool publicAccess = (getter != null && getter.IsPublic) || (setter != null && setter.IsPublic);

                bool hasSerializeAttr = p.GetCustomAttributes(typeof(SerializeField), false).Length > 0;
                // Accept only non-indexer properties:
                bool isIndexer = p.GetIndexParameters().Length > 0;

                return !isIndexer && (publicAccess || hasSerializeAttr);
            }

            return false;
        }

        var visitedGameObjects = new HashSet<int>();
        void TraverseGameObject(GameObject go)
        {
            if (go == null)
                return;

            int id = go.GetInstanceID();
            if (!visitedGameObjects.Add(id))
                return; // avoid cycles

            var comps = go.GetComponents<MonoBehaviour>();
            foreach (var mb in comps)
                InspectComponent(mb);

            var audioSources = go.GetComponents<AudioSource>();
            foreach (var audioSource in audioSources)
            {
                if (!audioSource)
                    continue;

                Type audioType = audioSource.GetType();

                if (!s_inspectionCache.TryGetValue(audioType, out var cachedInfo))
                {
                    cachedInfo = new CachedTypeInfo();

                    // properties
                    var audioSourceProps = audioSource.GetType().GetProperties(bf);
                    foreach (var prop in audioSourceProps)
                    {
                        if (prop.GetIndexParameters().Length > 0)
                            continue;

                        if (!IsSerializedMember(prop))
                            continue;

                        var getter = prop.GetGetMethod(true);
                        var setter = prop.GetSetMethod(true);

                        var t = prop.PropertyType;
                        var cm = new CachedMember
                        {
                            Member = prop,
                            MemberType = t,
                            Name = prop.Name,
                            IsProperty = true,
                            HasGetterAndSetter = (getter != null && setter != null)
                        };

                        if (IsAudioClipType(prop.PropertyType))
                        {
                            if (getter == null || setter == null)
                                continue;

                            cm.IsAudioSingle = t == typeof(AudioClip);
                            cm.IsAudioArray = t == typeof(AudioClip[]);
                            cm.IsAudioList = (t.IsGenericType &&
                                              t.GetGenericTypeDefinition() == typeof(List<>) &&
                                              t.GetGenericArguments()[0] == typeof(AudioClip));
                            cachedInfo.Members.Add(cm);

                            string key = audioSource.GetInstanceID().ToString() + "|" + prop.Name;
                            if (seen.Add(key))
                            {
                                audioMembers.Add(new MemberRef { Member = prop, Target = audioSource });
                            }
                        }
                    }

                    s_inspectionCache[audioType] = cachedInfo;
                }
                else if (cachedInfo.Members.Count > 0)
                {
                    foreach (var cm in cachedInfo.Members)
                    {
                        if (!cm.IsAudioSingle && !cm.IsAudioArray && !cm.IsAudioList)
                            continue;

                        if (!cm.HasGetterAndSetter)
                            continue;

                        string key = audioSource.GetInstanceID().ToString() + "|" + cm.Name;
                        if (seen.Add(key))
                        {
                            audioMembers.Add(new MemberRef { Member = cm.Member, Target = audioSource });
                        }
                    }
                }
            }

            var goTransform = go.transform;
            for (int i = 0; i < goTransform.childCount; i++)
                TraverseGameObject(goTransform.GetChild(i).gameObject);
        }

        void InspectComponent(MonoBehaviour component)
        {
            if (component == null)
                return;

            Type mbType = component.GetType();

            // Check cache
            if (!s_inspectionCache.TryGetValue(mbType, out var cachedInfo))
            {
                // build cache for this type
                cachedInfo = new CachedTypeInfo();

                var members = mbType.GetMembers(bf);
                foreach (var member in members)
                {
                    if (!IsSerializedMember(member))
                        continue;

                    if (member is FieldInfo field)
                    {
                        var t = field.FieldType;
                        var cm = new CachedMember
                        {
                            Member = member,
                            MemberType = t,
                            Name = field.Name,
                            IsProperty = false
                        };


                        var value = field.GetValue(component);
                        var type = field.FieldType;

                        if (IsAudioClipType(field.FieldType))
                        {
                            cm.IsAudioSingle = t == typeof(AudioClip);
                            cm.IsAudioArray = t == typeof(AudioClip[]);
                            cm.IsAudioList = (t.IsGenericType &&
                                              t.GetGenericTypeDefinition() == typeof(List<>) &&
                                              t.GetGenericArguments()[0] == typeof(AudioClip));
                            cachedInfo.Members.Add(cm);

                            string baseKeyPrefix = component.GetInstanceID().ToString();
                            string key = baseKeyPrefix + "|" + field.Name;
                            if (seen.Add(key))
                            {
                                audioMembers.Add(new MemberRef { Member = field, Target = component });
                            }
                        }
                        else if (type == typeof(GameObject))
                        {
                            cm.IsGameObject = true;
                            cachedInfo.Members.Add(cm);

                            TraverseGameObject(value as GameObject);
                        }
                        else if (type == typeof(GameObject[]))
                        {
                            cm.IsGameObjectArray = true;
                            cachedInfo.Members.Add(cm);

                            var array = value as GameObject[];
                            if (array == null)
                                continue;

                            foreach (var go in array)
                                TraverseGameObject(go);
                        }
                        else if (IsGameObjectList(type))
                        {
                            cm.IsGameObjectList = true;
                            cachedInfo.Members.Add(cm);

                            var list = value as List<GameObject>;
                            if (list == null)
                                continue;

                            foreach (var go in list)
                                TraverseGameObject(go);
                        }
                        else if (IsAssetReference(type))
                        {
                            cm.IsAssetReference = true;
                            cachedInfo.Members.Add(cm);

                            // try to load referenced asset and traverse if it's a GameObject
                            try
                            {
                                if (TryLoadAssetObject(value, out var assetObj) && assetObj is GameObject goAsset)
                                {
                                    TraverseGameObject(goAsset);
                                }
                            }
                            catch { }
                        }
                        else if (type.IsArray && IsAssetReference(type.GetElementType()))
                        {
                            cm.IsAssetReferenceArray = true;
                            cachedInfo.Members.Add(cm);

                            var arr = value as Array;
                            if (arr != null)
                            {
                                foreach (var ar in arr)
                                {
                                    try
                                    {
                                        if (TryLoadAssetObject(ar, out var assetObj) && assetObj is GameObject goA)
                                            TraverseGameObject(goA);
                                    }
                                    catch { }
                                }
                            }
                        }
                        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && IsAssetReference(type.GetGenericArguments()[0]))
                        {
                            cm.IsAssetReferenceList = true;
                            cachedInfo.Members.Add(cm);

                            var list = value as System.Collections.IEnumerable;
                            if (list != null)
                            {
                                foreach (var ar in list)
                                {
                                    try
                                    {
                                        if (TryLoadAssetObject(ar, out var assetObj) && assetObj is GameObject goA)
                                            TraverseGameObject(goA);
                                    }
                                    catch { }
                                }
                            }
                        }
                        else if (typeof(UnityEngine.Component).IsAssignableFrom(type))
                        {
                            cm.IsComponent = true;
                            cachedInfo.Members.Add(cm);

                            var comp = value as UnityEngine.Component;
                            if (comp != null)
                                TraverseGameObject(comp.gameObject);
                        }
                        else if (type.IsArray && typeof(UnityEngine.Component).IsAssignableFrom(type.GetElementType()))
                        {
                            cm.IsComponentArray = true;
                            cachedInfo.Members.Add(cm);

                            var arr = value as Array;
                            if (arr != null)
                            {
                                foreach (var obj in arr)
                                {
                                    var comp = obj as UnityEngine.Component;
                                    if (comp != null)
                                        TraverseGameObject(comp.gameObject);
                                }
                            }
                        }

                        else if (IsComponentList(t))
                        {
                            cm.IsComponentList = true;
                            cachedInfo.Members.Add(cm);

                            var listObj = value as System.Collections.IList;
                            if (listObj != null)
                            {
                                foreach (var o in listObj)
                                    if (o is UnityEngine.Component c)
                                        TraverseGameObject(c.gameObject);
                            }
                        }
                    }
                    else if (member is PropertyInfo prop)
                    {
                        if (prop.GetIndexParameters().Length > 0)
                            continue;

                        var getter = prop.GetGetMethod(true);
                        var setter = prop.GetSetMethod(true);

                        var t = prop.PropertyType;
                        var cm = new CachedMember
                        {
                            Member = member,
                            MemberType = prop.PropertyType,
                            Name = prop.Name,
                            IsProperty = true,
                            HasGetterAndSetter = (getter != null && setter != null)
                        };


                        if (IsAudioClipType(prop.PropertyType))
                        {
                            if (getter == null || setter == null)
                                continue;

                            cm.IsAudioSingle = t == typeof(AudioClip);
                            cm.IsAudioArray = t == typeof(AudioClip[]);
                            cm.IsAudioList = (t.IsGenericType &&
                                              t.GetGenericTypeDefinition() == typeof(List<>) &&
                                              t.GetGenericArguments()[0] == typeof(AudioClip));
                            cachedInfo.Members.Add(cm);

                            string baseKeyPrefix = component.GetInstanceID().ToString();
                            string key = baseKeyPrefix + "|" + prop.Name;
                            if (seen.Add(key))
                            {
                                audioMembers.Add(new MemberRef { Member = prop, Target = component });
                            }
                            continue;
                        }

                        object value;
                        try
                        {
                            value = prop.GetValue(component, null);
                        }
                        catch
                        {
                            continue;
                        }

                        var type = prop.PropertyType;

                        if (getter == null)
                            continue;

                        if (type == typeof(GameObject))
                        {
                            cm.IsGameObject = true;
                            cachedInfo.Members.Add(cm);

                            TraverseGameObject(value as GameObject);
                        }
                        else if (type == typeof(GameObject[]))
                        {
                            cm.IsGameObjectArray = true;
                            cachedInfo.Members.Add(cm);

                            var array = value as GameObject[];
                            if (array == null)
                                continue;

                            foreach (var go in array)
                                TraverseGameObject(go);
                        }
                        else if (IsGameObjectList(type))
                        {
                            cm.IsGameObjectList = true;
                            cachedInfo.Members.Add(cm);

                            var list = value as List<GameObject>;
                            if (list == null)
                                continue;

                            foreach (var go in list)
                                TraverseGameObject(go);
                        }
                        else if (IsAssetReference(type))
                        {
                            cm.IsAssetReference = true;
                            cachedInfo.Members.Add(cm);

                            // try to load referenced asset and traverse if it's a GameObject
                            try
                            {
                                if (TryLoadAssetObject(value, out var assetObj) && assetObj is GameObject goAsset)
                                {
                                    TraverseGameObject(goAsset);
                                }
                            }
                            catch { }
                        }
                        else if (type.IsArray && IsAssetReference(type.GetElementType()))
                        {
                            cm.IsAssetReferenceArray = true;
                            cachedInfo.Members.Add(cm);

                            var arr = value as Array;
                            if (arr != null)
                            {
                                foreach (var ar in arr)
                                {
                                    try
                                    {
                                        if (TryLoadAssetObject(ar, out var assetObj) && assetObj is GameObject goA)
                                            TraverseGameObject(goA);
                                    }
                                    catch { }
                                }
                            }
                        }
                        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && IsAssetReference(type.GetGenericArguments()[0]))
                        {
                            cm.IsAssetReferenceList = true;
                            cachedInfo.Members.Add(cm);

                            var list = value as System.Collections.IEnumerable;
                            if (list != null)
                            {
                                foreach (var ar in list)
                                {
                                    try
                                    {
                                        if (TryLoadAssetObject(ar, out var assetObj) && assetObj is GameObject goA)
                                            TraverseGameObject(goA);
                                    }
                                    catch { }
                                }
                            }
                        }
                        else if (typeof(UnityEngine.Component).IsAssignableFrom(type))
                        {
                            cm.IsComponent = true;
                            cachedInfo.Members.Add(cm);

                            var comp = value as UnityEngine.Component;
                            if (comp != null)
                                TraverseGameObject(comp.gameObject);
                        }
                        else if (type.IsArray && typeof(UnityEngine.Component).IsAssignableFrom(type.GetElementType()))
                        {
                            cm.IsComponentArray = true;
                            cachedInfo.Members.Add(cm);

                            var arr = value as Array;
                            if (arr != null)
                            {
                                foreach (var obj in arr)
                                {
                                    var comp = obj as UnityEngine.Component;
                                    if (comp != null)
                                        TraverseGameObject(comp.gameObject);
                                }
                            }
                        }

                        else if (IsComponentList(t))
                        {
                            cm.IsComponentList = true;
                            cachedInfo.Members.Add(cm);

                            var listObj = value as System.Collections.IList;
                            if (listObj != null)
                            {
                                foreach (var o in listObj)
                                    if (o is UnityEngine.Component c)
                                        TraverseGameObject(c.gameObject);
                            }
                        }
                    }
                }

                s_inspectionCache[mbType] = cachedInfo;
            }
            else if (cachedInfo.Members.Count > 0)
            {
                foreach (var cm in cachedInfo.Members)
                {
                    try
                    {
                        if (cm.IsAudioSingle || cm.IsAudioArray || cm.IsAudioList)
                        {
                            string key = component.GetInstanceID().ToString() + "|" + cm.Name;
                            if (seen.Add(key))
                            {
                                audioMembers.Add(new MemberRef { Member = cm.Member, Target = component });
                            }
                            continue;
                        }

                        object value = null;
                        if (cm.IsProperty)
                        {
                            var p = (PropertyInfo)cm.Member;
                            var getter = p.GetGetMethod(true);
                            if (getter == null)
                                continue;

                            try { value = getter.Invoke(component, null); } catch { continue; }
                        }
                        else
                        {
                            var f = (FieldInfo)cm.Member;
                            try { value = f.GetValue(component); } catch { continue; }
                        }

                        if (cm.IsGameObject)
                        {
                            TraverseGameObject(value as GameObject);
                        }
                        else if (cm.IsGameObjectArray)
                        {
                            var arr = value as GameObject[];
                            if (arr != null)
                            {
                                foreach (var go1 in arr)
                                    TraverseGameObject(go1);
                            }
                        }
                        else if (cm.IsGameObjectList)
                        {
                            var list = value as List<GameObject>;
                            if (list != null)
                            {
                                foreach (var go1 in list)
                                    TraverseGameObject(go1);
                            }
                        }
                        else if (cm.IsAssetReference)
                        {
                            try
                            {
                                if (TryLoadAssetObject(value, out var assetObj) && assetObj is GameObject goA)
                                    TraverseGameObject(goA);
                            }
                            catch { }
                        }
                        else if (cm.IsAssetReferenceArray)
                        {
                            var arr = value as Array;
                            if (arr != null)
                            {
                                foreach (var ar in arr)
                                    try
                                    {
                                        if (TryLoadAssetObject(ar, out var assetObj) && assetObj is GameObject goA)
                                            TraverseGameObject(goA);
                                    }
                                    catch { }
                            }
                        }
                        else if (cm.IsAssetReferenceList)
                        {
                            var list = value as System.Collections.IEnumerable;
                            if (list != null)
                            {
                                foreach (var ar in list)
                                    try
                                    {
                                        if (TryLoadAssetObject(ar, out var assetObj) && assetObj is GameObject goA)
                                            TraverseGameObject(goA);
                                    }
                                    catch { }
                            }
                        }
                        else if (cm.IsComponent)
                        {
                            var comp = value as UnityEngine.Component;
                            if (comp != null)
                                TraverseGameObject(comp.gameObject);
                        }
                        else if (cm.IsComponentArray)
                        {
                            var arr = value as Array;
                            if (arr != null)
                            {
                                foreach (var obj in arr)
                                {
                                    var comp = obj as UnityEngine.Component;
                                    if (comp != null)
                                        TraverseGameObject(comp.gameObject);
                                }
                            }
                        }

                        else if (cm.IsComponentList)
                        {
                            var listObj = value as System.Collections.IList;
                            if (listObj != null)
                            {
                                foreach (var o in listObj)
                                    if (o is UnityEngine.Component c)
                                        TraverseGameObject(c.gameObject);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        // Find GameObject references on all discovered MonoBehaviours and traverse them
        try
        {
            var allBehaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var mb in allBehaviours)
            {
                InspectComponent(mb);
            }
        }
        catch (Exception)
        {
            // ignore
        }

        // Also check AudioSource-specific members (AudioSource has a clip property)
        try
        {
            var audioSources = UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var audioSource in audioSources)
            {
                if (audioSource == null)
                    continue;

                Type audioType = audioSource.GetType();

                if (!s_inspectionCache.TryGetValue(audioType, out var cachedInfo))
                {
                    cachedInfo = new CachedTypeInfo();

                    // properties
                    var audioSourceProps = audioSource.GetType().GetProperties(bf);
                    foreach (var prop in audioSourceProps)
                    {
                        if (prop.GetIndexParameters().Length > 0)
                            continue;

                        if (!IsSerializedMember(prop))
                            continue;

                        var getter = prop.GetGetMethod(true);
                        var setter = prop.GetSetMethod(true);

                        // Accept property only if it is audio type or is a reference type we traverse
                        var t = prop.PropertyType;
                        var cm = new CachedMember
                        {
                            Member = prop,
                            MemberType = t,
                            Name = prop.Name,
                            IsProperty = true,
                            HasGetterAndSetter = (getter != null && setter != null)
                        };

                        if (IsAudioClipType(prop.PropertyType))
                        {
                            if (getter == null || setter == null)
                                continue;

                            cm.IsAudioSingle = t == typeof(AudioClip);
                            cm.IsAudioArray = t == typeof(AudioClip[]);
                            cm.IsAudioList = (t.IsGenericType &&
                                              t.GetGenericTypeDefinition() == typeof(List<>) &&
                                              t.GetGenericArguments()[0] == typeof(AudioClip));
                            cachedInfo.Members.Add(cm);

                            string key = audioSource.GetInstanceID().ToString() + "|" + prop.Name;
                            if (seen.Add(key))
                            {
                                audioMembers.Add(new MemberRef { Member = prop, Target = audioSource });
                            }
                        }
                    }

                    s_inspectionCache[audioType] = cachedInfo;
                }
                else if (cachedInfo.Members.Count > 0)
                {
                    foreach (var cm in cachedInfo.Members)
                    {
                        if (!cm.IsAudioSingle && !cm.IsAudioArray && !cm.IsAudioList)
                            continue;

                        if (!cm.HasGetterAndSetter)
                            continue;

                        string key = audioSource.GetInstanceID().ToString() + "|" + cm.Name;
                        if (seen.Add(key))
                        {
                            audioMembers.Add(new MemberRef { Member = cm.Member, Target = audioSource });
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // ignore
        }

        return audioMembers;
    }

    // Try to load an asset from an AssetReference-like object using reflection.
    // This avoids a hard dependency on Addressables. The method will attempt to call
    // LoadAssetAsync<T>(), WaitForCompletion() and then Release the handle if possible.
    private static bool TryLoadAssetObject(object assetReferenceInstance, out UnityEngine.Object asset)
    {
        asset = null;
        if (assetReferenceInstance == null)
            return false;

        try
        {
            var arType = assetReferenceInstance.GetType();

            // ---- Addressables safety checks ----
            var runtimeKeyProp = arType.GetProperty("RuntimeKey", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (runtimeKeyProp != null)
            {
                var key = runtimeKeyProp.GetValue(assetReferenceInstance);

                if (key == null)
                    return false;

                if (key is string s && string.IsNullOrWhiteSpace(s))
                    return false;

                if (key is System.Collections.IEnumerable && !(key is string))
                    return false;
            }

            var validMethod = arType.GetMethod("RuntimeKeyIsValid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (validMethod != null)
            {
                bool ok = (bool)validMethod.Invoke(assetReferenceInstance, null);
                if (!ok)
                    return false;
            }
            // -----------------------------------


            // 1) If the AssetReference already exposes an OperationHandle, prefer using its Result
            var opProp = arType.GetProperty("OperationHandle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (opProp != null)
            {
                try
                {
                    var existingHandle = opProp.GetValue(assetReferenceInstance);
                    if (existingHandle != null)
                    {
                        var handleType = existingHandle.GetType();
                        // Try Result first
                        var resultProp = handleType.GetProperty("Result", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (resultProp != null)
                        {
                            var res = resultProp.GetValue(existingHandle) as UnityEngine.Object;
                            if (res != null)
                            {
                                asset = res;
                                return true;
                            }
                        }

                        // If Result not available yet, try WaitForCompletion on the existing handle
                        var waitMethodExisting = handleType.GetMethod("WaitForCompletion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (waitMethodExisting != null)
                        {
                            var res = waitMethodExisting.Invoke(existingHandle, null) as UnityEngine.Object;
                            if (res != null)
                            {
                                asset = res;
                                return true;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            // 2) No exposed handle/result -> create our own load handle (but remember we created it so we can release it)
            bool createdHandle = false;
            object loadObj = null;

            MethodInfo loadGeneric = null;
            foreach (var m in arType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (m.Name == "LoadAssetAsync" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0)
                {
                    loadGeneric = m;
                    break;
                }
            }

            if (loadGeneric == null)
                return false;

            loadObj = loadGeneric.MakeGenericMethod(typeof(UnityEngine.Object)).Invoke(assetReferenceInstance, null);
            if (loadObj == null)
                return false;

            createdHandle = true;
            var handleType2 = loadObj.GetType();

            var waitMethod = handleType2.GetMethod("WaitForCompletion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (waitMethod != null)
            {
                var result = waitMethod.Invoke(loadObj, null);
                asset = result as UnityEngine.Object;
            }
            else
            {
                var resultProp2 = handleType2.GetProperty("Result", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (resultProp2 != null)
                    asset = resultProp2.GetValue(loadObj) as UnityEngine.Object;
            }

            // Release only handles we created (be conservative — only release when a compatible API exists)
            if (createdHandle && loadObj != null)
            {
                try
                {
                    var handleTypeLocal = loadObj.GetType();

                    // If the handle itself exposes an instance Release(), call it.
                    var releaseInst = handleTypeLocal.GetMethod("Release", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (releaseInst != null)
                    {
                        try { releaseInst.Invoke(loadObj, null); } catch { }
                    }
                    else
                    {
                        // Otherwise try to find a static Addressables.Release overload that accepts this handle type
                        var addrType = Type.GetType("UnityEngine.AddressableAssets.Addressables, Unity.Addressables")
                                       ?? Type.GetType("UnityEngine.AddressableAssets.Addressables");
                        if (addrType != null)
                        {
                            MethodInfo relMatch = null;
                            // 1) Prefer overloads whose parameter type is assignable from our handle type.
                            // 2) Do NOT select the overload that takes System.Object (Addressables.Release(object))
                            foreach (var rm in addrType.GetMethods(BindingFlags.Static | BindingFlags.Public))
                            {
                                if (rm.Name != "Release") continue;
                                var pars = rm.GetParameters();
                                if (pars.Length != 1) continue;
                                var pType = pars[0].ParameterType;

                                if (pType == typeof(object))
                                    continue; // explicitly skip Release(object) — it's unsafe here

                                if (pType.IsAssignableFrom(handleTypeLocal))
                                {
                                    relMatch = rm;
                                    break;
                                }

                                // fallback: if parameter type name suggests AsyncOperationHandle (covers struct generic variants)
                                if (relMatch == null && pType.Name.Contains("AsyncOperationHandle", StringComparison.OrdinalIgnoreCase))
                                {
                                    // double-check assignability via name because generic struct types are tricky via reflection
                                    relMatch = rm;
                                    // don't break here; prefer a direct assignable match if found later
                                }
                            }

                            if (relMatch != null)
                            {
                                try
                                {
                                    relMatch.Invoke(null, new object[] { loadObj });
                                }
                                catch (TargetInvocationException)
                                {
                                    // Swallow Addressables-specific exceptions that can occur during release (e.g. InvalidKeyException)
                                    // Do not rethrow — releasing is best-effort here to avoid double-load errors.
                                }
                                catch { }
                            }
                            // If no safe Release overload was found (i.e. only Release(object) exists), skip releasing.
                        }
                    }
                }
                catch { }
            }

            return asset != null;
        }
        catch
        {
            return false;
        }
    }

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        if (n < 2)
            return list;

        // preserve original values to detect fixed points after shuffle
        T[] original = new T[n];
        for (int i = 0; i < n; i++)
            original[i] = list[i];

        // shuffle
        int m = n;
        while (m > 1)
        {
            m--;
            int k = RandomForMe.Next(m + 1);
            T tmp = list[k];
            list[k] = list[m];
            list[m] = tmp;
        }

        // collect indices that ended up equal to their original value
        var fixedIdx = new List<int>();
        for (int i = 0; i < n; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], original[i]))
                fixedIdx.Add(i);
        }

        if (fixedIdx.Count == 0)
            return list;

        if (fixedIdx.Count == 1)
        {
            // swap the single fixed-point with a random other index
            int idx = fixedIdx[0];
            int j = RandomForMe.Next(n - 1);
            if (j >= idx)
                j++; // ensure j != idx

            T tmp = list[idx];
            list[idx] = list[j];
            list[j] = tmp;
            return list;
        }

        // rotate values among fixed-point indices (repairs all those fixed points)
        T firstVal = list[fixedIdx[0]];
        for (int t = 0; t < fixedIdx.Count - 1; t++)
        {
            list[fixedIdx[t]] = list[fixedIdx[t + 1]];
        }
        list[fixedIdx[fixedIdx.Count - 1]] = firstVal;

        // fallback
        bool stillFixed = false;
        for (int i = 0; i < n; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], original[i]))
            {
                stillFixed = true;
                break;
            }
        }

        if (stillFixed)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int j = RandomForMe.Next(i);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }

        return list;
    }
}

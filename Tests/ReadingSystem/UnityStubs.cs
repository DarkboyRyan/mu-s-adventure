using System;
using System.Collections.Generic;
namespace UnityEngine {
 [AttributeUsage(AttributeTargets.Class)] public class DefaultExecutionOrder:Attribute {public DefaultExecutionOrder(int n){}}
 [AttributeUsage(AttributeTargets.Class)] public class RequireComponent:Attribute {public RequireComponent(Type t){}}
 [AttributeUsage(AttributeTargets.Class)] public class CreateAssetMenu:Attribute {public string fileName;public string menuName;}
 [AttributeUsage(AttributeTargets.Field)] public class Header:Attribute {public Header(string s){}}
 [AttributeUsage(AttributeTargets.Field)] public class TextArea:Attribute {public TextArea(int a,int b){}}
 public class Object {static int next;readonly int id=++next; public int GetInstanceID()=>id;public static void Destroy(Object o){} }
 public class GameObject:Object { public bool activeSelf=true;public bool activeInHierarchy=>activeSelf;public string tag="Untagged";public readonly Transform transform=new Transform();public readonly Dictionary<Type,object> components=new();public void SetActive(bool v)=>activeSelf=v; }
 public class Component:Object {public GameObject gameObject=new();public Transform transform=>gameObject.transform;public bool CompareTag(string t)=>gameObject.tag==t;public T GetComponent<T>() where T:class=>gameObject.components.TryGetValue(typeof(T),out var x)?x as T:null;}
 public class MonoBehaviour:Component {public bool enabled=true;public bool isActiveAndEnabled=>enabled && gameObject.activeInHierarchy;}
 public class ScriptableObject:Object {}
 public class Transform:Object {public Vector3 position;}
 public struct Vector3 {public float x,y,z;public Vector3(float x,float y,float z){this.x=x;this.y=y;this.z=z;}public float sqrMagnitude=>x*x+y*y+z*z;public static Vector3 operator -(Vector3 a,Vector3 b)=>new(a.x-b.x,a.y-b.y,a.z-b.z);}
 public class Rigidbody2D:Component {}
 public class Collider2D:Component {public bool enabled=true;public Rigidbody2D attachedRigidbody;}
 public static class Time {public static float timeScale=1;public static int frameCount=1;}
 public enum KeyCode {E,A,D,LeftArrow,RightArrow,Escape}
 public static class Input {public static HashSet<KeyCode> keys=new();public static bool GetKeyDown(KeyCode k)=>keys.Contains(k);}
 public static class Mathf {public static int Clamp(int v,int a,int b)=>Math.Clamp(v,a,b);}
 public static class Debug {public static void LogWarning(string s,Object o){} }
}
namespace TMPro {public class TMP_Text:UnityEngine.MonoBehaviour {public string text;}}
public class PlayerLadderClimb:UnityEngine.MonoBehaviour {}
public class CameraFollow:UnityEngine.MonoBehaviour {public void SetTarget(UnityEngine.Transform t){} }

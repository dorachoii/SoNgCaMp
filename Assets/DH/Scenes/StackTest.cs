using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackTest : MonoBehaviour
{
    public static StackTest instance;
    Stack<GameObject> stack = new Stack<GameObject>();
    public ButtonTest CurrnetPage;

    private void Awake()
    {
        instance = this;
    }
    public void Test(GameObject test) {
        stack.Push(test);
    }

    public void pop() {
        //내거 지우고
        GameObject go = stack.Pop();
        go.gameObject.SetActive(false);
        //다음꺼 키고 
        GameObject go2 = stack.Pop();
        go2.gameObject.SetActive(true);
    }
}

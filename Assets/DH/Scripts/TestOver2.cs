using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOver2 : TestOver1
{
    private void Start()
    {
        base.Attack();
        TestOver1 kk = this;
        kk.Attack();
    }
    public override void Attack()
    {
        Debug.Log("이것이 성기사의 힘이다!");
    }
}

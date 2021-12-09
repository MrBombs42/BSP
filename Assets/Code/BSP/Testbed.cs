using System;
using BSP.Assets.Code.InterTreeTest;
using UnityEngine;
    

namespace BSP.Assets.Code.BSP
{
    public class Testbed : MonoBehaviour {    
    
        private ITree<int> intergerTree;

        private void Awake(){

            DebugTree();
        }

        private void DebugTree(){
             intergerTree = new IntegerTree();

            intergerTree.Insert(10, 10);
            intergerTree.Insert(5, 5);
            intergerTree.Insert(6, 6);
            intergerTree.Insert(4, 4);
            intergerTree.Insert(2, 2);
            intergerTree.Insert(15, 15);

            var restult = intergerTree.Search(intergerTree.Root, 2);
            UnityEngine.Debug.Log(restult.Id);

            intergerTree.Print();  
        }

        private int Expoente(int num, int exp){
            if(exp == 0){
                return 1;
            }

            return num * Expoente(num, --exp);
        }
    }
}

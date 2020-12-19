using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<T, U> : IComparable<Pair<double, Pair<int, int>>>
{
    public Pair() //empty constructor
    {
    }

    public Pair(T first, U second) //generic types
    {
        this.first = first;
        this.second = second;
    }

    //BAD PRACTICE!
    /*
     * Ideally we should NOT have to convert a generic T to a double in our CompareTo,
     * rather, we should have a separate pPair class that we use for storing Pair<double,Pair<int,int>
     * However, since only pPair variants of the Pair object will need to be compared by SortedSet in the A* algo,
     * we know that we will never have a case where the Generic "T" is NOT a double.
     * */
    public int CompareTo(Pair<double, Pair<int, int>> obj) // allows us to compare f values of Pair objects in the sortedset in A* algo
    {
            Pair<double,Pair<int,int>> p = obj; 

            double self_first = Convert.ToDouble(this.first);

            if (self_first < p.first)
            {
                return -1;
            }
            else if (self_first > p.first)
            {
                return 1;
            }
            return 0;

    }

    //setters and getters
    public T first { get; set; }
    public U second { get; set; }
};

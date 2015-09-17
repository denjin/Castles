using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T> {
	T[] items;
	int currentItemCount;

	public Heap(int _maxHeapSize) {
		items = new T[_maxHeapSize];
	}

	public void Add(T item) {
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}

	void SortUp(T item) {
		int parentIndex = (items.HeapIndex - 1) / 2;

		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				Swap(item, parentItem);
			} else {
				break;
			}
		}
	}

	void Swap(T itemA, T itemB) {
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itamA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}
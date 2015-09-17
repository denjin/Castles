using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T> {
	T[] items;
	int currentItemCount;

	public Heap(int _maxHeapSize) {
		items = new T[_maxHeapSize];
	}

	public int Count {
		get {
			return currentItemCount;
		}
	}

	/**
	 * Forces an update of the position of the supplied item in the heap
	 * @param T item the item to sort
	 */
	public void UpdateItem(T item) {
		SortUp(item);
	}

	/**
	 * Checks if heap contains the supplied item
	 * @param T item the item to check
	 */
	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	/**
	 * Adds a new item to the heap
	 * @param T item the item to add
	 */
	public void Add(T item) {
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}

	/**
	 * Removes the first item from the heap and returns it
	 */
	public T RemoveFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	/**
	 * Goes from top to bottom, sorting the supplied item with it's children
	 * @param  T item the item to sort
	 */
	void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;
			//check if item has a child on the left
			if (childIndexLeft < currentItemCount) {
				//set the item to swap as the left child
				swapIndex = childIndexLeft;
				//check if item has a child on the right
				if (childIndexRight < currentItemCount) {
					//check if child on the right has a lower priority than the child on the left
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						//set the item to swap as the right child
						swapIndex = childIndexRight;
					}
					//check if item has a higher priority than highest child
					if (item.CompareTo(items[swapIndex]) < 0) {
						//swap these items in the heap
						Swap(item, items[swapIndex]);
					} else {
						return;
					}
				}
			} else {
				return;
			}
		}
	}

	/**
	 * Goes from bottom to top, sorting the supplied item with it's parent
	 * @param T item the item to sort
	 */
	void SortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				Swap(item, parentItem);
			} else {
				break;
			}
			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	/**
	 * Swaps two items
	 * @param T itemA
	 * @param T itemB
	 */
	void Swap(T itemA, T itemB) {
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
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
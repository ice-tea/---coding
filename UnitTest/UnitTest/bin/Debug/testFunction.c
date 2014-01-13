#define C 2.9979e8

int add(int x,int y)
{
	return x + y;
}
int main()
{
	//@ #length1 = km
	int length1;
	//@ #length2 = m;
	int length2;
	
	//error
	add( length1 , length2);
	return 0;
}
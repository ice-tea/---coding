#define C 2.9979e8

int main()
{
	//@ #length1 = m;
	int length1;
	//@ #length2 = km;
	int length2;
	//@ #temp = ?;
	int temp;
	if( length1 < length2)
	{
		temp = length2;
	}
	else
	{
		temp = length1;
	}
	return 0;
}
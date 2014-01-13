
#define C 2.9979e8

int main()
{

	//@ #a = km;
	int a = 10;
	//@ #b = s;
	float b = 100;
	//@ #c = (#a)/(#b);
	float c = a/b;
	//@ #10=m; #10=s;
	//@ #d=?;
	int d = 10*10;
	
	if(c > b)
		return -1;
	return 0;
}
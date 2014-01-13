#define C 2.9979e8

//@ #s1 = m;
const int s1 = 100;
//@ #s2 = km;
const int s2 = 100;

int main()
{
	//@ #t = s;
	int t = 10;
	
	int v1 = 0;
	int v2 = 0;
	
	v1 = s1/t;
	v2 = s2/t;
	
	//error
	return v1>v2;
}
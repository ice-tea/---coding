#define C 2.9979e8

//@ #{A}a = m;
//@ #{A}b = km;
struct A
{
	int a;
	int b;
};
int main()
{
	struct A myA;
	int length1;
	int length2;

	length1 = myA.a;
	length2 = myA.b;
	
	//error
	return length1 + length2;
}
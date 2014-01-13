const double C=2.9979e8;
char* MYUNIT2[] = { "#{A}a" , "m" };
char* MYUNIT3[] = { "#{A}b" , "km" };
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
	return length1 + length2;
}


const double C=2.9979e8;
char* MYUNIT2[] = { "#s1" , "m" };
const int s1 = 100;
char* MYUNIT4[] = { "#s2" , "km" };
const int s2 = 100;
int main()
{
char* MYUNIT9[] = { "#t" , "s" };
	int t = 10;
	int v1 = 0;
	int v2 = 0;
	v1 = s1/t;
	v2 = s2/t;
	return v1>v2;
}


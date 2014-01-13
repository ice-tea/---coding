#define C 2.9979e8

//@ #length = m;
const int length = 100;

int main()
{
	//@ #squar = ?;
	int squar;
	
	squar = length;
	squar = squar * length;
	
	//error
	if(squar < length)
		return 0;
	else
		return 0;
}
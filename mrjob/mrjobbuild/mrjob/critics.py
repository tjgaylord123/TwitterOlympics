import  sys
for  line in sys.stdin:
	nyt_results = newyorktimes['results']
	res = nyt_results[0]
	thousand = res['thousand_best']
	critics = res['critics_pick']
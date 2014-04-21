from mrjob.job import MRJob
from mrjob.protocol import JSONValueProtocol


class MRCriticScoreJob(MRJob):

    def mapper(self, _, line):
    	i=0
    	j=0
    	results = nyt["results"]
    	res = results[0]
    	cp = res["critics_pick"]
    	if cp=='Y':
    		i=1
    	else:
    		j=1
    	yield "Y", i
        yield "N", j
        yield "lines", 1

    def reducer(self, key, values):
        yield key, sum(values)


if __name__ == '__main__':
    MRCriticScoreJob.run()
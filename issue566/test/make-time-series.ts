import * as monitoring from '@google-cloud/monitoring';


describe('make-time-series', () => {
    it('compiles', () => {
        const monitoringClient = new monitoring.MetricServiceClient({
            projectId: 'my-project-id',
            credentials: require('../gcpServiceAccount.json'),
        });
        monitoringClient.createTimeSeries({
            
        });
    });
});




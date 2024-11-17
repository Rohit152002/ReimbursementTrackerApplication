<template>
    <div>
            {{ requestData }}
    </div>
</template>

<script>
import { getRequests } from '@/scripts/Request';

    export default {
        name:"ReimbursementRequest",
        data()
        {
            return {
                requestData:{}
            }
        },

        async mounted()
        {
            const res = await getRequests()
            console.log(res.data.data)
            this.requestData=res.data.data;
        }
    }
</script>

<style scoped>

</style>

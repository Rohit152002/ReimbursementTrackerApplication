<template>
    <div>
        <p>{{ userName }}</p>
        <p>{{ email }}</p>

    </div>
</template>

<script>
import { getUserProfile } from '@/scripts/User';

    export default {
        name:"UserProfile",
        data(){
            return {
                userName:'',
                email:"",
                department:"",
            }
        },
        async mounted()
        {
            const res = await getUserProfile();
            console.log(res);
            this.userName=res.data.userName;
            this.email=res.data.email;
        }
    }
</script>

<style>

</style>

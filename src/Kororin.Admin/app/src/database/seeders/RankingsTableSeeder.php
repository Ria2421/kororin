<?php

namespace Database\Seeders;

use App\Models\Ranking;
use Illuminate\Database\Seeder;

class RankingsTableSeeder extends Seeder
{
    public function run(): void
    {
        Ranking::create([
            'user_id' => 1,
            'stage_id' => 1,
            'clear_time' => 22.5,
        ]);
        Ranking::create([
            'user_id' => 2,
            'stage_id' => 1,
            'clear_time' => 18.5,
        ]);
        Ranking::create([
            'user_id' => 3,
            'stage_id' => 1,
            'clear_time' => 28.5,
        ]);
        Ranking::create([
            'user_id' => 4,
            'stage_id' => 1,
            'clear_time' => 15.5,
        ]);
        Ranking::create([
            'user_id' => 5,
            'stage_id' => 1,
            'clear_time' => 30.5,
        ]);


        Ranking::create([
            'user_id' => 1,
            'stage_id' => 2,
            'clear_time' => 30.8,
        ]);
        Ranking::create([
            'user_id' => 2,
            'stage_id' => 2,
            'clear_time' => 36.8,
        ]);
        Ranking::create([
            'user_id' => 3,
            'stage_id' => 2,
            'clear_time' => 28.8,
        ]);
        Ranking::create([
            'user_id' => 4,
            'stage_id' => 2,
            'clear_time' => 20.8,
        ]);
        Ranking::create([
            'user_id' => 5,
            'stage_id' => 2,
            'clear_time' => 36.8,
        ]);

        
        Ranking::create([
            'user_id' => 1,
            'stage_id' => 3,
            'clear_time' => 28.6,
        ]);
        Ranking::create([
            'user_id' => 2,
            'stage_id' => 3,
            'clear_time' => 33.6,
        ]);
        Ranking::create([
            'user_id' => 3,
            'stage_id' => 3,
            'clear_time' => 28.3,
        ]);
        Ranking::create([
            'user_id' => 4,
            'stage_id' => 3,
            'clear_time' => 24.6,
        ]);
        Ranking::create([
            'user_id' => 5,
            'stage_id' => 3,
            'clear_time' => 36.6,
        ]);
    }
}
